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
	[TestCase(loremIpsum, 00, int.MaxValue, "remove 2147483647 characters starting from char. #0", "")]
	[TestCase(loremIpsum, 11, 107, "remove 107 characters starting from char. #11", "Lorem ipsum")]
	[TestCase(loremIpsum, 26, int.MaxValue, "remove 2147483647 characters starting from char. #26", "Lorem ipsum dolor sit amet")]
	[TestCase(neverForget, 0, 17, "remove 17 characters starting from char. #0", "never forget kindnesses")]
	public void Test8(string input, int startIndex, int count, string description, string expected)
	{
		RemoveAction removeAction = new(new FixedIndexFinder(startIndex), count);
		Assert.AreEqual(description, removeAction.Description);
		Assert.AreEqual(expected, removeAction.Run(input));
	}

	[Test]
	[TestCase(quickBrownFox, -2, "remove the last 2 characters", "the quick brown fox jumps over the lazy do")]
	[TestCase(quickBrownFox, -3, "remove the last 3 characters", "the quick brown fox jumps over the lazy d")]
	[TestCase(quickBrownFox, -5, "remove the last 5 characters", "the quick brown fox jumps over the lazy")]
	public void Test9(string input, int count, string description, string expected)
	{
		RemoveAction removeAction = new RemoveAction(new EndIndexFinder(), count);
		Assert.AreEqual(description, removeAction.Description);
		Assert.AreEqual(expected, removeAction.Run(input));
		Assert.Pass();
	}


	private static void Test(string expected, string input, IIndexFinder startIndexFinder, IIndexFinder endIndexFinder)
	{
		Assert.AreEqual(expected, new RemoveAction(startIndexFinder, endIndexFinder).Run(input));
	}


	[Test]
	[TestCase(0, 5, "remove file name")]
	[TestCase(0, 6, "remove all characters")]
	[TestCase(5, 6, "remove file extension")]
	[TestCase(1, 3, @"remove characters from char. #3 to after ""dark""")]
	public void TestDescription(int startIndexFinderIndex, int endIndexFinderIndex, string expected)
	{
		Assert.AreEqual(expected, new RemoveAction(finders[startIndexFinderIndex], finders[endIndexFinderIndex]).Description);
	}
}