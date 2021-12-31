using NUnit.Framework;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

public sealed class Test_ReplaceAction
{
	#region Fields

	private const string quickBrownFox = "the quick brown fox jumps over the lazy dog.";
	private const string fiveBoxingWizards = "The five boxing wizards jump quickly.";
	private const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.";
	private const string neverForget = "Forget injuries, never forget kindnesses";
	private const string quickBrownNinjaCat = "the quick brown ninja cat jumps over the lazy dog.";
	private const string regex1 = @",[^\.]*(?=,)";

	#endregion


	[Test]
	[TestCase(quickBrownFox, "fox", "ninja cat", false, false, quickBrownNinjaCat)]
	[TestCase(quickBrownFox, "fox", "ninja cat", true, false, quickBrownNinjaCat)]
	[TestCase(quickBrownFox, "Fox", "ninja cat", false, false, quickBrownFox)]
	[TestCase(quickBrownFox, "Fox", "ninja cat", true, false, quickBrownNinjaCat)]
	[TestCase(fiveBoxingWizards, " ", ".", false, false, "The.five.boxing.wizards.jump.quickly.")]
	[TestCase(loremIpsum, regex1, null, false, true, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, lacinia ipsum.")]
	[TestCase(neverForget, regex1, "", false, true, neverForget)]
	public void Test(string input, string oldString, string newString, bool ignoreCase, bool useRegex, string expected)
	{
		Assert.AreEqual(expected, new ReplaceAction(oldString, newString, ignoreCase, useRegex).Run(input));
		Assert.Pass();
	}
}