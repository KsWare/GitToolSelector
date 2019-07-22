using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.ToolSelector
{
    class ConfFile
    {
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
