using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.GitToolSelector;
using NUnit.Framework;

namespace KsWare.GitToolSelectorTests
{
	[TestFixture]
	public class ProgramTests
	{
		private string fileA, fileB, externalParser, semanticMergeTool;

		[SetUp]
		public void Setup()
		{
			File.Copy(TestHelper.GetTestDataPath("GitToolSelector.conf"),
				Path.Combine(Environment.CurrentDirectory, "GitToolSelector.conf"));
			fileA = TestHelper.GetTestDataPath("Resource1.resx");
			fileB = TestHelper.GetTestDataPath("Resource2.resx");
			externalParser = "C:\\Program Files\\SemanticMerge\\External\\XmlSemanticParser.exe";
			semanticMergeTool = "C:\\Program Files\\SemanticMerge\\semanticmergetool.exe";
		}

		[Test]
		public void RunTest()
		{
			var args = new[] {"-tool", "diff", "-s", fileA, "-d", fileB};

			var sut = Program.Run(args);
			Assert.That(sut.FileName, Is.EqualTo(semanticMergeTool));
			Assert.That(sut.Arguments, Is.EqualTo($"-s \"{fileA}\" -d \"{fileB}\" -ep \"{externalParser}\""));
		}

		[Test]
		public void RunDiffCsTest()
		{
			fileA = @"C:\a.cs";
			fileB = @"C:\b.cs";
			var args = new[] {"-tool", "diff", "-s", fileA, "-d", fileB};

			var sut = Program.Run(args);
			Assert.That(sut.FileName, Is.EqualTo(semanticMergeTool));
			Assert.That(sut.Arguments, Is.EqualTo($"-s \"{fileA}\" -d \"{fileB}\""));
		}
	}
}