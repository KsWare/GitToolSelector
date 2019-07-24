using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KsWare.MergeToolSelector
{
    class ConfFile
    {
	    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(IniFile));
        private IniFile _iniFile;
        private readonly Dictionary<string,string> _filterToSection=new Dictionary<string, string>();

        public ConfFile(string configFile)
        {
	        var locations = new List<string>
	        {
				GetFileFromUserFolder(),
		        GetFileFromAssemblyLocation()
	        };
			if(!string.IsNullOrEmpty(configFile)) locations.Insert(0, configFile);

	        foreach (var path in locations)
	        {
		        if (File.Exists(path))
		        {
			        FullPath = path;
					break;
		        }
	        }
            if (!File.Exists(FullPath))
            {
				Log.Error($"Configuration file not found! Search path: {string.Join("\n",locations)}");
            }

            
            _iniFile = new IniFile(FullPath);
            foreach (var sectionName in _iniFile.SectionNames)
            {
                var filters = sectionName.Split(';');
                foreach (var filter in filters)
                {
                    _filterToSection.Add(filter,sectionName);
                }
            }
        }


        public string FullPath { get; private set; }

        internal string GetFileFromAssemblyLocation()
        {
	        var location = Assembly.GetExecutingAssembly().Location;
	        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
	        var path = Path.Combine(Path.GetDirectoryName(location), assemblyName + ".conf");
	        return path;
        }

        internal string GetFileFromUserFolder()
        {
	        var path = Path.Combine("%localappdata%", "KsWare", "MergeToolSelector", "MergeToolSelector.conf");
	        return Environment.ExpandEnvironmentVariables(path);
        }

        public string GetExtensionFromOverrides(string fileName)
        {
            var ext = _iniFile.ReadMatch("Overrides", fileName);
            return ext;
        }

        public string GetValue(string filter, string valueName)
        {
            if (!_filterToSection.TryGetValue(filter, out var section))
            {
                return null;
            }
            return _iniFile.Read(section, valueName);
        }
    }
}
