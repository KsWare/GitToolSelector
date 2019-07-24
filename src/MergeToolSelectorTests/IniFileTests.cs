using KsWare.MergeToolSelector;
using NUnit.Framework;

namespace KsWare.MergeToolSelectorTests
{
    [TestFixture]
    public class IniFileTests
    {
        [Test]
        public void TestMethod1()
        {
            var n= TestHelper.GetTestDataPath("MergeToolSelector.conf");
            var sut = new IniFile(n);
            Assert.That(sut.Read("test1", "a"),Is.EqualTo("\"C:\\Path\\foo.exe\" \"foo\" \"bar\""));
            Assert.That(sut.Read("test1", "b"),Is.EqualTo("\"C:\\Path\\foo.exe\" \"foo\" bar"));
            Assert.That(sut.Read("test1", "c"),Is.EqualTo("\"C:\\Path\\foo.exe\" foo bar"));
            Assert.That(sut.Read("test1", "d"),Is.EqualTo("C:\\Path\\foo.exe foo bar"));
            Assert.That(sut.Read("test1", "e"),Is.EqualTo("C:\\Path\\foo.exe \"foo\" bar"));
										
            Assert.That(sut.Read("test1", @"C:\path 2\file.exe"),Is.EqualTo("C:\\path 2\\file.exe"));
            Assert.That(sut.Read("test1", "foo bar"),Is.EqualTo("foo bar"));
            Assert.That(sut.Read("test1", "foo \"3\" bar"),Is.EqualTo("foo \"3\" bar"));
            Assert.That(sut.Read("test1", "foo\\\"3\" bar "),Is.EqualTo("foo\\\"3\" bar"));
        }
    }
}