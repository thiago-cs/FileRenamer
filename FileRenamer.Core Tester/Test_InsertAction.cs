using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_InsertAction
{
	#region Insertion tests

	[Test]
	[TestCase(quickBrownFox, "Maybe ")]
	[TestCase("", "Empty")]
	public void AddToBeginning(string input, string addend)
	{
		Assert.AreEqual(addend + input, new InsertAction(new BeginningIndexFinder(), addend).Run(input));
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
		Assert.AreEqual(expected, new InsertAction(new SubstringIndexFinder(reference, isBefore, false, false), addend).Run(input));
		Assert.Pass();
	}

	#endregion


	#region Description tests

	[Test]
	[TestCase(0, "this", @"insert ""this"" at the beginning")]
	[TestCase(6, "that", @"insert ""that"" at the end")]
	[TestCase(1, "third", @"insert ""third"" after char. #3")]
	[TestCase(2, "Jesse", @"insert ""Jesse"" before ""sunset""")]
	[TestCase(3, "mint", @"insert ""mint"" after ""dark""")]
	[TestCase(4, "!", @"insert ""!"" after the expression ""(Hi|Hello) kitty""")]
	[TestCase(5, ".part", @"insert "".part"" before file's extension")]
	public void TestDescriptionBeginning(int indexFinderIndex, string value, string expected)
	{
		Assert.AreEqual(expected, new InsertAction(finders[indexFinderIndex], value).Description);
	}

	#endregion
}