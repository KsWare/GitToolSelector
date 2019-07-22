﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KsWare.ToolSelector
{
    class IniFile
    {
        public static string DefaultExtension { get; set; } = ".ini";
        private static readonly Regex SectionRegex=new Regex(@"\s*\[(?<section>[^\]]*)\].*", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex ValueRegex=new Regex(@"^\s*(?<key>([a-z][^=\s]*))\s*=\s*(?<value>.*)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        string _path;
        string _exe = Assembly.GetExecutingAssembly().GetName().Name;
        private readonly Dictionary<string,Section> _sections=new Dictionary<string,Section>();

        public IniFile(string path = null)
        {
            _path = new FileInfo(path ?? _exe + DefaultExtension).FullName.ToString();
            var section = new Section("");
            _sections.Add("",section);
            using (var stream = File.OpenText(_path))
            {
                string line;
                while ((line=stream.ReadLine())!=null)
                {
                    var sectionMatch = SectionRegex.Match(line);
                    if (sectionMatch.Success)
                    {
                        var sectionName = sectionMatch.Groups["section"].Value;
                        section=new Section(sectionName);
                        _sections.Add(sectionName.ToLowerInvariant(),section);
                        continue;
                    }

                    var valueMatch = ValueRegex.Match(line);
                    if (valueMatch.Success)
                    {
                        section.Add(valueMatch.Groups["key"].Value, valueMatch.Groups["value"].Value.Trim());
                    }

                }
            }
        }

        public IEnumerable<string> SectionNames => _sections.Select(s=>s.Value.SectionName);

        public string Read(string sectionName, string key)
        {
            if (!_sections.TryGetValue(sectionName.ToLowerInvariant(), out var section))
            {
                return null;
            }

            if(!section.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
        }
        internal class Section
        {
            private readonly Dictionary<string,string> _values=new Dictionary<string, string>();
            public string SectionName { get; }

            public Section(string sectionName)
            {
                SectionName = sectionName;
            }

            public void Add(string key, string value)
            {
                _values.Add(key.ToLowerInvariant(), value);
            }

            public bool TryGetValue(string key, out string value)
            {
                return _values.TryGetValue(key.ToLowerInvariant(), out value);
            }
        }
    }
}