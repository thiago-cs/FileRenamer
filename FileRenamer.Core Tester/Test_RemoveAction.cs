using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_RemoveAction
{
	[Test]
	public void Test1()
	{
		Test("", quickBrownFox, new BeginningIndexFinder(), new EndIndexFinder());
		Assert.Pass();
	}

	[Test]
	public void Test2()
	{
		Test("The five boxing wizards", fiveBoxingWizards, new SubstringIndexFinder("wizards", false, false, false), new EndIndexFinder());
		Assert.Pass();
	}

	[Test]
	public void Test3()
	{
		Test("Lorem ipsum dolor sit amet. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.",
			loremIpsum,
			new SubstringIndexFinder(", ", true, false, false),
			new SubstringIndexFinder(".", true, false, false));
		Assert.Pass();
	}

	[Test]
	public void Test4()
	{
		Test("never forget kindnesses", neverForget, new BeginningIndexFinder(), new SubstringIndexFinder(", ", false, false, false));
		Assert.Pass();
	}

	[Test]
	public void Test5()
	{
		Test(quickBrownFox, quickBrownFox, new SubstringIndexFinder("dog", true, false, false), new SubstringIndexFinder("fox", true, false, false));
		Assert.Pass();
	}

	[Test]
	public void Test6()
	{
		Test(loremIpsum, loremIpsum, new SubstringIndexFinder("Hello", true, false, false), new SubstringIndexFinder(".", true, false, false));
		Assert.Pass();
	}

	[Test]
	public void Test7()
	{
		Test(loremIpsum, loremIpsum, new SubstringIndexFinder("o", true, false, false), new SubstringIndexFinder("World!", true, false, false));
		Assert.Pass();
	}

	[Test]
	[TestCase(loremIpsum, 00, int.MaxValue, "")]
	[TestCase(loremIpsum, 11, 107, "Lorem ipsum")]
	[TestCase(loremIpsum, 26, int.MaxValue, "Lorem ipsum dolor sit amet")]
	[TestCase(neverForget, 0, 17, "never forget kindnesses")]
	public void Test8(string input, int startIndex, int count, string expected)
	{
		Assert.AreEqual(expected, new RemoveAction(new FixedIndexFinder(startIndex), count).Run(input));
		Assert.Pass();
	}

	[Test]
	[TestCase(quickBrownFox, -2, "the quick brown fox jumps over the lazy do")]
	[TestCase(quickBrownFox, -3, "the quick brown fox jumps over the lazy d")]
	[TestCase(quickBrownFox, -5, "the quick brown fox jumps over the lazy")]
	public void Test9(string input, int count, string expected)
	{
		Assert.AreEqual(expected, new RemoveAction(new EndIndexFinder(), count).Run(input));
		Assert.Pass();
	}


	private static void Test(string expected, string input, IIndexFinder startIndexFinder, IIndexFinder endIndexFinder)
	{
		Assert.AreEqual(expected, new RemoveAction(startIndexFinder, endIndexFinder).Run(input));
	}
}