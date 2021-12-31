using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

public sealed class Test_RemoveAction
{
	#region Fields

	private const string quickBrownFox = "the quick brown fox jumps over the lazy dog.";
	private const string fiveBoxingWizards = "The five boxing wizards jump quickly.";
	private const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.";
	private const string neverForget = "Forget injuries, never forget kindnesses";

	#endregion


	[Test]
	public void Test1()
	{
		Test("", quickBrownFox, new BeginningIndexFinder(), new EndIndexFinder());
		Assert.Pass();
	}

	[Test]
	public void Test2()
	{
		Test("The five boxing wizards", fiveBoxingWizards, new SubstringIndexFinder("wizards", false), new EndIndexFinder());
		Assert.Pass();
	}

	[Test]
	public void Test3()
	{
		Test("Lorem ipsum dolor sit amet. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.",
			loremIpsum,
			new SubstringIndexFinder(", ", true),
			new SubstringIndexFinder(".", true));
		Assert.Pass();
	}

	[Test]
	public void Test4()
	{
		Test("never forget kindnesses", neverForget, new BeginningIndexFinder(), new SubstringIndexFinder(", ", false));
		Assert.Pass();
	}

	[Test]
	public void Test5()
	{
		Test(quickBrownFox, quickBrownFox, new SubstringIndexFinder("dog", true), new SubstringIndexFinder("fox", true));
		Assert.Pass();
	}

	[Test]
	public void Test6()
	{
		Test(loremIpsum, loremIpsum, new SubstringIndexFinder("Hello", true), new SubstringIndexFinder(".", true));
		Assert.Pass();
	}

	[Test]
	public void Test7()
	{
		Test(loremIpsum, loremIpsum, new SubstringIndexFinder("o", true), new SubstringIndexFinder("World!", true));
		Assert.Pass();
	}

	[Test]
	[TestCase(loremIpsum, 00, int.MaxValue, "")]
	[TestCase(loremIpsum, 11, 107, "Lorem ipsum")]
	[TestCase(loremIpsum, 26, int.MaxValue, "Lorem ipsum dolor sit amet")]
	[TestCase(neverForget, 0, 17, "never forget kindnesses")]
	public void Test8(string input, int startIndex, int count, string expected)
	{
		Test(expected, input, startIndex, count);
		Assert.Pass();
	}


	private static void Test(string expected, string input, IIndexFinder startIndexFinder, IIndexFinder endIndexFinder)
	{
		Assert.AreEqual(expected, new RemoveAction(startIndexFinder, endIndexFinder).Run(input));
	}

	private static void Test(string expected, string input, int startIndex, int count)
	{
		Assert.AreEqual(expected, new RemoveAction(new FixedIndexFinder(startIndex), count).Run(input));
	}
}