﻿using NUnit.Framework;
using FileRenamer.Core.Actions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_ReplaceAction
{
	[Test]
	[TestCase(quickBrownFox, "fox", "ninja cat", false, false, quickBrownNinjaCat)]
	[TestCase(quickBrownFox, "fox", "ninja cat", true, false, quickBrownNinjaCat)]
	[TestCase(quickBrownFox, "Fox", "ninja cat", false, false, quickBrownFox)]
	[TestCase(quickBrownFox, "Fox", "ninja cat", true, false, quickBrownNinjaCat)]
	[TestCase(fiveBoxingWizards, " ", ".", false, false, "The.five.boxing.wizards.jump.quickly.")]
	[TestCase(loremIpsum, regex1, null, false, true, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, lacinia ipsum.")]
	[TestCase(neverForget, regex1, "", false, true, neverForget)]
	public void TestRun(string input, string oldString, string newString, bool ignoreCase, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(oldString, newString, ignoreCase, useRegex).Run(input));
	}


	[Test]
	[TestCase("fox", "ninja cat", false, @"replace ""fox"" with ""ninja cat""")]
	[TestCase(regex1, "new text", true, @"replace the expression "",[^\.]*(?=,)"" with ""new text""")]
	public void TestDescription(string oldString, string newString, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(oldString, newString, false, useRegex).Description);
	}
}