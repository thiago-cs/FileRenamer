using NUnit.Framework;
using FileRenamer.Core.Indices;
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
	public void TestWithoutBounds(string input, string oldString, string newString, bool ignoreCase, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(oldString, newString, ignoreCase, useRegex).Run(input));
	}


	[Test]
	[TestCase(quickBrownFox, 05, 10, "fox", "ninja cat", false, false, quickBrownFox)]
	[TestCase(quickBrownFox, 15, 20, "fox", "ninja cat", false, false, quickBrownNinjaCat)]
	[TestCase(quickBrownFox, 09, 16, /*language=regex*/ @"\s", ".", false, true, "the quick.brown.fox jumps over the lazy dog.")]
	[TestCase("sun of a beach", 10, 12, /*language=regex*/ @"\w", "*", false, true, "sun of a b**ch")]
	public void TestWithBounds(string input, int start, int end, string oldString, string newString, bool ignoreCase, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(new FixedIndexFinder(start), new FixedIndexFinder(end), oldString, newString, ignoreCase, useRegex).Run(input));
	}


	[Test]
	[TestCase("fox", "ninja cat", false, @"replace ""fox"" with ""ninja cat""")]
	[TestCase(regex1, "new text", true, @"replace the expression "",[^\.]*(?=,)"" with ""new text""")]
	public void TestDescriptionWithoutBounds(string oldString, string newString, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(oldString, newString, false, useRegex).Description);
	}


	[Test]
	[TestCase(0, 5, "fox", "ninja cat", false, @"replace ""fox"" within file name with ""ninja cat""")]
	[TestCase(2, 4, regex1, "new text", true, @"replace the expression "",[^\.]*(?=,)"" within characters from before ""sunset"" to after the expression ""(Hi|Hello) kitty"" with ""new text""")]
	public void TestDescriptionWithBounds(int i1, int i2, string oldString, string newString, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(finders[i1], finders[i2], oldString, newString, false, useRegex).Description);
	}
}