using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;
using FileRenamer.Core.Extensions;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_ToCaseAction
{
	[Test]
	[TestCase("new document.txt", true, TextCasing.TitleCase, "New Document.txt")]
	[TestCase("new document.txt", false, TextCasing.UpperCase, "new document.TXT")]
	[TestCase("Area.effect", true, TextCasing.LowerCase, "area.effect")]
	public void FileNameTest(string fileName, bool convertName, TextCasing casing, string expected)
	{
		FileExtensionIndexFinder extensionIndexFinder = new();
		IIndexFinder startIndexFinder;
		IIndexFinder endIndexFinder;

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

		Assert.AreEqual(expected, new ToCaseAction(startIndexFinder, endIndexFinder, casing).Run(fileName));
		Assert.Pass();
	}
}