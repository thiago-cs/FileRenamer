using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

public sealed class Test_InsertAction
{
	#region Fields

	private const string quickBrownFox = "the quick brown fox jumps over the lazy dog.";
	private const string fiveBoxingWizards = "The five boxing wizards jump quickly.";
	private const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.";
	private const string neverForget = "Forget injuries, never forget kindnesses";

	private readonly IIndexFinder[] finders =
	{
		new BeginningIndexFinder(),
		new FixedIndexFinder(3),
		new SubstringIndexFinder("sunset", true, false, false),
		new SubstringIndexFinder("dark", false, false, false),
		new SubstringIndexFinder("(Hi|Hello) kitty", false, false, true),
		new FileExtensionIndexFinder(),
		new EndIndexFinder(),
	};

	#endregion


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