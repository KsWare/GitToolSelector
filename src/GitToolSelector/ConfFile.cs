using System.Collections.Generic;

namespace KsWare.GitToolSelector
{
    class ConfFile
    {
	    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(IniFile));
        private IniFile _iniFile;
        private readonly Dictionary<string,string> _filterToSection=new Dictionary<string, string>();

        public ConfFile()
        {
            IniFile.DefaultExtension = ".conf";
            _iniFile = new IniFile();
            foreach (var sectionName in _iniFile.SectionNames)
            {
                var filters = sectionName.Split(';');
                foreach (var filter in filters)
                {
                    _filterToSection.Add(filter,sectionName);
                }
            }
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
