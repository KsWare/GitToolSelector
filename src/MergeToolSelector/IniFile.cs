using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KsWare.MergeToolSelector
{
    internal class IniFile
    {
	    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(IniFile));
        private static readonly Regex SectionRegex=new Regex(@"\s*\[(?<section>[^\]]*)\].*", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex ValueRegex=new Regex(@"^\s*(?<key>(("".*(?<!\\)"")|([^#;][^=]*(?!\s))))\s*=\s*(?<value>.*)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        string _path;
        private readonly Dictionary<string,Section> _sections=new Dictionary<string,Section>(StringComparer.OrdinalIgnoreCase);

        public IniFile(string path)
        {
            _path = Path.GetFullPath(path);
			Log.Info($"Open: {_path}");
            var lineNumber = 0;
            var section = new Section("",lineNumber);
            _sections.Add("",section);
            using (var stream = File.OpenText(_path))
            {
                string line;
                while ((line=stream.ReadLine())!=null)
                {
	                lineNumber++;
                    var sectionMatch = SectionRegex.Match(line);
                    if (sectionMatch.Success)
                    {
                        var sectionName = sectionMatch.Groups["section"].Value;
                        section=new Section(sectionName, lineNumber);
                        _sections.Add(sectionName,section);
                        continue;
                    }

                    var valueMatch = ValueRegex.Match(line);
                    if (valueMatch.Success)
                    {
	                    var key = valueMatch.Groups["key"].Value.Trim();
	                    if (key.StartsWith("\""))
	                    {
							// \"→"  \\→\ 
							key = key.Substring(1, key.Length - 2).Replace("\\\"", "\"").Replace(@"\\", @"\");
	                    }
                        section.Add(key, valueMatch.Groups["value"].Value.Trim());
						continue;
                    }

                    line = line.Trim();

                    if(line.StartsWith("#") || line.StartsWith(";")) // comment
                    {
	                    continue;
                    }

                    if(string.IsNullOrWhiteSpace(line)) // empty line
                    {
	                    continue;
                    }

					Log.Warn($"Unknown content in line {lineNumber} skipped: {line}");
                }
            }
        }

        public IEnumerable<string> SectionNames => _sections.Select(s=>s.Value.SectionName);

        public string Read(string sectionName, string key)
        {
            if (!_sections.TryGetValue(sectionName, out var section))
            {
                return null;
            }

            if(!section.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
        }

        public string ReadMatch(string sectionName, string key)
        {
	        if (!_sections.TryGetValue(sectionName, out var section))
	        {
		        return null;
	        }

	        foreach (var k in section.Keys)
	        {
		        var pattern = CreateRegex(k);
		        if (Regex.IsMatch(key, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
		        {
			        return section.Values[k];
		        }
	        }

	        return null;
        }

        internal static string CreateRegex(string searchPattern) //TODO move and make internal for test
        {
	        var sp = searchPattern.Replace("*", "~STAR~").Replace("?", "~QUESTION~");
	        sp = Regex.Escape(sp);
	        var regex=sp.Replace("~STAR~",".*").Replace("~QUESTION~", ".?");
	        return regex;
        }

        internal class Section
        {
            private readonly Dictionary<string, string> _values=new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            public string SectionName { get; }

			public int LineNumber { get; }

            public IEnumerable<string> Keys => _values.Keys;

            public Dictionary<string, string> Values => _values;

            public Section(string sectionName, int lineNumber)
            {
                SectionName = sectionName;
                LineNumber = lineNumber;
            }

            public void Add(string key, string value)
            {
                _values.Add(key, value);
            }

            public bool TryGetValue(string key, out string value)
            {
                return _values.TryGetValue(key, out value);
            }

			public string this[string key] => _values[key];
        }
    }
}
