using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;
using FileRenamer.Core.Extensions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_ToCaseAction
{
	[Test]
	[TestCase(0, true, TextCasing.TitleCase, "New Document.txt")]
	[TestCase(0, false, TextCasing.UpperCase, "new document.TXT")]
	[TestCase(1, true, TextCasing.LowerCase, "area.effect")]
	public void FileNameTest(int fileNameIndex, bool convertName, TextCasing casing, string expected)
	{
		IIndexFinder startIndexFinder;
		IIndexFinder endIndexFinder;
		FileExtensionIndexFinder extensionIndexFinder = new();

		if (convertName)
		{
			startIndexFinder = new BeginningIndexFinder();
			endIndexFinder = extensionIndexFinder;
		}
		else
		{
			startIndexFinder = extensionIndexFinder;
			endIndexFinder  = new EndIndexFinder();
		}

		Assert.AreEqual(expected, new ToCaseAction(startIndexFinder, endIndexFinder, casing).Run(fileNames[fileNameIndex]));
	}


	[Test]
	[TestCase(0, 5, TextCasing.LowerCase, "convert file name to lowercase")]
	[TestCase(0, 6, TextCasing.TitleCase, "convert all characters to title case")]
	[TestCase(5, 6, TextCasing.UpperCase, "convert file extension to uppercase")]
	[TestCase(2, 3, TextCasing.SentenceCase, @"convert characters from before ""sunset"" to after ""dark"" to sentence case")]
	public void TestDescription(int startIndexFinderIndex, int endIndexFinderIndex, TextCasing casing, string expected)
	{
		Assert.AreEqual(expected, new ToCaseAction(finders[startIndexFinderIndex], finders[endIndexFinderIndex], casing).Description);
	}
}