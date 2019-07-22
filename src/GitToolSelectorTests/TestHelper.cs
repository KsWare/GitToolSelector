using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.ToolSelectorTests
{
    static class TestHelper
    {
        public static string GetTestDataPath(string relativPath)
        {
            var asspath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(asspath, "TestData", relativPath);
        }
    }
}
