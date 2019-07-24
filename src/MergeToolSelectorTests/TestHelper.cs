using System.IO;
using System.Reflection;

namespace KsWare.MergeToolSelectorTests
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
