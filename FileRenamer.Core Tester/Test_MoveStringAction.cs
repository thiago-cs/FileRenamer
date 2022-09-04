using NUnit.Framework;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.Extensions;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_MoveStringAction
{
	[Test]
	[TestCase("my name is thiago c", "thiago c?s?", true, true, -3, "my name thiago cis ")]
	[TestCase("my name is thiago s", "thiago c?s?", true, true, 1, "my name is thiago s")]
	[TestCase("my name is thiago cs", " ..", true, true, 9, "myme is thi naago cs")]
	[TestCase("my name is cs thiago!", " cs", true, false, 7, "my name is thiago cs!")]
	public void FileNameTest(string input, string oldString, bool ignoreCase, bool useRegex, int count, string expected)
	{
		MoveStringAction action = new(oldString, ignoreCase, useRegex, count);

		Assert.AreEqual(expected, action.Run(input));
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