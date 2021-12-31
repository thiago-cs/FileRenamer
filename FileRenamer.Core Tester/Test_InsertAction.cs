using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

public class Test_InsertAction
{
	#region Fields

	private const string quickBrownFox = "the quick brown fox jumps over the lazy dog.";
	private const string fiveBoxingWizards = "The five boxing wizards jump quickly.";
	private const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.";
	private const string neverForget = "Forget injuries, never forget kindnesses";

	#endregion


	[Test]
	[TestCase(quickBrownFox, "Maybe ")]
	[TestCase("", "Empty")]
	public void AddToBeginning(string input, string addend)
	{
		Assert.AreEqual(addend + input, new InsertAction(new BeginingIndexFinder(), addend).Run(input));
		Assert.Pass();
	}

	[Test]
	[TestCase(neverForget, ".")]
	public void AddToEnd(string input, string addend)
	{
		Assert.AreEqual(input + addend, new InsertAction(new EndIndexFinder(), addend).Run(input));
		Assert.Pass();
	}

	[Test]
	[TestCase(fiveBoxingWizards, "wizards", false, " do not", "The five boxing wizards do not jump quickly.")]
	[TestCase(loremIpsum, "Etiam ", false, "m", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam mat sem consectetur, egestas velit vitae, lacinia ipsum.")]
	[TestCase(quickBrownFox, "dog", true, "old ", "the quick brown fox jumps over the lazy old dog.")]
	public void InsertInTheMiddle(string input, string reference, bool isBefore, string addend, string expected)
	{
		Assert.AreEqual(expected, new InsertAction(new SubstringIndexFinder(reference, isBefore), addend).Run(input));
		Assert.Pass();
	}
}