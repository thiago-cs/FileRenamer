using NUnit.Framework;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.Extensions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_ChangeStringCaseAction
{
	[Test]
	[TestCase("my name is thiago c", "thiago c?s?", true, true, TextCasing.TitleCase, "my name is Thiago C")]
	[TestCase("my name is thiago s", "thiago c?s?", true, true, TextCasing.UpperCase, "my name is THIAGO S")]
	[TestCase("my name is thiago cs", "thiago c?s?", true, true, TextCasing.UpperCase, "my name is THIAGO CS")]
	[TestCase("my name is thiago cs", ".*", true, true, TextCasing.TitleCaseIgnoreCommonWords, "My Name Is Thiago Cs")]
	public void FileNameTest(string input, string oldString, bool ignoreCase, bool useRegex, TextCasing textCase, string expected)
	{
		ChangeStringCaseAction action = new(oldString, ignoreCase, useRegex, textCase);
		JobTarget target = new(new FileMock(input), 0);
		action.Run(target, NoContext);

		Assert.AreEqual(expected, target.NewFileName);
	}


	[Test]
	[TestCase(true,  true,  TextCasing.LowerCase, @"change all occurrences of the expression ""thiago c?s"" to lowercase (ignore case)")]
	[TestCase(true,  false, TextCasing.UpperCase, @"change all occurrences of ""thiago c?s"" to uppercase (ignore case)")]
	[TestCase(false, true,  TextCasing.SentenceCase, @"change all occurrences of the expression ""thiago c?s"" to sentence case")]
	[TestCase(false, false, TextCasing.TitleCase, @"change all occurrences of ""thiago c?s"" to title case")]
	[TestCase(false, false, TextCasing.TitleCaseIgnoreCommonWords, @"change all occurrences of ""thiago c?s"" to title case (ignore common words)")]
	public void TestDescription(bool ignoreCase, bool useRegex, TextCasing textCase, string expected)
	{
		string oldString = "thiago c?s";
		Assert.AreEqual(expected, new ChangeStringCaseAction(oldString, ignoreCase, useRegex, textCase).Description);
	}
}