using NUnit.Framework;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture(Description = "Testing the Remove action that removes part of the file/folder name.")]
public sealed class Test_RemoveAction
{
	[Test]
	public void Test1()
	{
		Test("", quickBrownFox, new BeginningIndex(), new EndIndex());
	}

	[Test]
	public void Test2()
	{
		Test("The five boxing wizards", fiveBoxingWizards, new SubstringIndex("wizards", false, false, false), new EndIndex());
	}

	[Test]
	public void Test3()
	{
		Test("Lorem ipsum dolor sit amet. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.",
			loremIpsum,
			new SubstringIndex(", ", true, false, false),
			new SubstringIndex(".", true, false, false));
	}

	[Test]
	public void Test4()
	{
		Test("never forget kindnesses", neverForget, new BeginningIndex(), new SubstringIndex(", ", false, false, false));
	}

	[Test]
	public void Test5()
	{
		Test(quickBrownFox, quickBrownFox, new SubstringIndex("dog", true, false, false), new SubstringIndex("fox", true, false, false));
	}

	[Test]
	public void Test6()
	{
		Test(loremIpsum, loremIpsum, new SubstringIndex("Hello", true, false, false), new SubstringIndex(".", true, false, false));
	}

	[Test]
	public void Test7()
	{
		Test(loremIpsum, loremIpsum, new SubstringIndex("o", true, false, false), new SubstringIndex("World!", true, false, false));
	}

	[Test]
	[TestCase(loremIpsum, 00, int.MaxValue, "remove 2147483647 characters after the 1st character", "")]
	[TestCase(loremIpsum, 11, 107, "remove 107 characters after the 11th character", "Lorem ipsum")]
	[TestCase(loremIpsum, 26, int.MaxValue, "remove 2147483647 characters after the 26th character", "Lorem ipsum dolor sit amet")]
	[TestCase(neverForget, 0, 17, "remove 17 characters after the 1st character", "never forget kindnesses")]
	public void Test8(string input, int startIndex, int count, string description, string expected)
	{
		RemoveAction removeAction = new(new FixedIndex(startIndex), count);

		Assert.AreEqual(description, removeAction.Description);

		JobTarget target = new(new FileMock(input), 0);
		removeAction.Run(target, NoContext);

		Assert.AreEqual(expected, target.NewFileName);
	}

	[Test]
	[TestCase(quickBrownFox, -2, "remove the last 2 characters", "the quick brown fox jumps over the lazy do")]
	[TestCase(quickBrownFox, -3, "remove the last 3 characters", "the quick brown fox jumps over the lazy d")]
	[TestCase(quickBrownFox, -5, "remove the last 5 characters", "the quick brown fox jumps over the lazy")]
	public void Test9(string input, int count, string description, string expected)
	{
		RemoveAction removeAction = new(new EndIndex(), count);

		Assert.AreEqual(description, removeAction.Description);

		JobTarget target = new(new FileMock(input), 0);
		removeAction.Run(target, NoContext);

		Assert.AreEqual(expected, target.NewFileName);
	}


	private static void Test(string expected, string input, IIndex startIndexFinder, IIndex endIndexFinder)
	{
		RemoveAction removeAction = new(startIndexFinder, endIndexFinder);
		JobTarget target = new(new FileMock(input), 0);
		removeAction.Run(target, NoContext);

		Assert.AreEqual(expected, target.NewFileName);
	}


	[Test(Description = "Testing the description for start and end indices:")]
	[TestCase(0, 5, "remove file name")]
	[TestCase(9, 5, "remove file name")]
	[TestCase(0, 6, "remove all characters")]
	[TestCase(9, 6, "remove all characters")]
	[TestCase(5, 6, "remove file extension")]
	[TestCase(1, 3, @"remove characters from 3rd character to after ""dark""")]
	[TestCase(8, 6, @"remove the last character")]
	[TestCase(7, 6, @"remove the last 2 characters")]
	public void TestRemoveRangeDescription(int startIndexFinderIndex, int endIndexFinderIndex, string expected)
	{
		RemoveAction removeAction = new(finders[startIndexFinderIndex], finders[endIndexFinderIndex]);
		Assert.AreEqual(expected, removeAction.Description);
	}


	[Test(Description = "Testing the description when removing chars before/after a text:")]
	[TestCase(2, 5, "remove 5 characters before \"sunset\"")]
	[TestCase(3, 5, "remove 5 characters after \"dark\"")]
	public void TestRemoveCountDescription(int startIndexFinderIndex, int count, string expected)
	{
		RemoveAction removeAction = new(finders[startIndexFinderIndex], count);
		Assert.AreEqual(expected, removeAction.Description);
	}
}