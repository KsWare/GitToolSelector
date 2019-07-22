using KsWare.GitToolSelector;
using NUnit.Framework;

namespace KsWare.GitToolSelectorTests
{
    [TestFixture]
    public class InitFileTests
    {
        [Test]
        public void TestMethod1()
        {
            var n= TestHelper.GetTestDataPath("ToolSelector.conf");
            var sut = new IniFile(n);
            Assert.That(sut.Read("a", "test1"),Is.EqualTo("\"C:\\Path\\foo.exe\" \"foo\" \"bar\""));
            Assert.That(sut.Read("b", "test1"),Is.EqualTo("\"C:\\Path\\foo.exe\" \"foo\" bar"));
            Assert.That(sut.Read("c", "test1"),Is.EqualTo("\"C:\\Path\\foo.exe\" foo bar"));
            Assert.That(sut.Read("d", "test1"),Is.EqualTo("C:\\Path\\foo.exe foo bar"));
            Assert.That(sut.Read("e", "test1"),Is.EqualTo("C:\\Path\\foo.exe \"foo\" bar"));
        }
    }
}
