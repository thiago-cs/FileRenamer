using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.Extensions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_ChangeRangeCaseAction
{
	[Test]
	[TestCase(0, true, TextCasing.TitleCase, "New Document.txt")]
	[TestCase(0, false, TextCasing.UpperCase, "new document.TXT")]
	[TestCase(1, true, TextCasing.LowerCase, "area.effect")]
	public void FileNameTest(int fileNameIndex, bool convertName, TextCasing casing, string expected)
	{
		IIndex startIndexFinder;
		IIndex endIndexFinder;
		FileExtensionIndex extensionIndexFinder = new();

		if (convertName)
		{
			startIndexFinder = new BeginningIndex();
			endIndexFinder = extensionIndexFinder;
		}
		else
		{
			startIndexFinder = extensionIndexFinder;
			endIndexFinder  = new EndIndex();
		}

		ChangeRangeCaseAction action = new(startIndexFinder, endIndexFinder, casing);

		Assert.AreEqual(expected, action.Run(fileNames[fileNameIndex]));
	}


	[Test]
	[TestCase(0, 5, TextCasing.LowerCase, "change file name to lowercase")]
	[TestCase(0, 6, TextCasing.TitleCase, "change all characters to title case")]
	[TestCase(5, 6, TextCasing.UpperCase, "change file extension to uppercase")]
	[TestCase(2, 3, TextCasing.SentenceCase, @"change characters from before ""sunset"" to after ""dark"" to sentence case")]
	[TestCase(0, 1, TextCasing.TitleCaseIgnoreCommonWords, @"change characters from beginning to 3rd character to title case (ignore common words)")]
	public void TestDescription(int startIndexFinderIndex, int endIndexFinderIndex, TextCasing casing, string expected)
	{
		Assert.AreEqual(expected, new ChangeRangeCaseAction(finders[startIndexFinderIndex], finders[endIndexFinderIndex], casing).Description);
	}
}