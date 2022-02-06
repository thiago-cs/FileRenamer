using NUnit.Framework;
using FileRenamer.Core.Indices;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_FindIndex
{
	[Test]
	public void BeginningIndexFinderTest()
	{
		if (testStrings == null)
		{
			Assert.Inconclusive($"This test could not be ran because the {nameof(testStrings)} variable is null.");
			return;
		}

		BeginningIndex indexFinder = new();

		for (int i = 0; i < testStrings.Length; i++)
			Assert.AreEqual(0, indexFinder.FindIn(testStrings[i]));

		Assert.Pass();
	}

	[Test]
	public void EndIndexFinderTest()
	{
		if (testStrings == null)
		{
			Assert.Inconclusive($"This test could not be ran because the {nameof(testStrings)} variable is null.");
			return;
		}

		EndIndex indexFinder = new();

		for (int i = 0; i < testStrings.Length; i++)
			Assert.AreEqual(testStrings[i].Length, indexFinder.FindIn(testStrings[i]));

		Assert.Pass();
	}

	[Test]
	public void FixedIndexFinderTest()
	{
		if (testStrings == null)
		{
			Assert.Inconclusive($"This test could not be ran because the {nameof(testStrings)} variable is null.");
			return;
		}

		System.Random random = new();

		for (int i = 0; i < testStrings.Length; i++)
		{
			int r = random.Next(100);
			Assert.AreEqual(r, new FixedIndex(r).FindIn(testStrings[i]));
		}

		Assert.Pass();
	}

	[Test]
	[TestCase(quickBrownFox, "fox", false, false, false, 19)]
	[TestCase(quickBrownFox, "Fox", false, true, false, 19)]
	[TestCase(quickBrownFox, "Fox", false, false, false, -1)]
	[TestCase(quickBrownFox, "dog", true, false, false, 40)]
	[TestCase(quickBrownFox, "Dog", true, true, false, 40)]
	[TestCase(quickBrownFox, "Dog", true, false, false, -1)]
	[TestCase(fiveBoxingWizards, "quick", true, false, false, 29)]
	[TestCase(fiveBoxingWizards, "quick", false, false, false, 34)]
	[TestCase(loremIpsum, "r", true, false, false, 2)]
	[TestCase(loremIpsum, "Lorem", true, false, false, 0)]
	[TestCase(loremIpsum, "lorem", false, false, false, -1)]
	[TestCase(loremIpsum, "lorem", false, true, false, 5)]
	[TestCase(loremIpsum, "lorem", true, false, false, -1)]
	[TestCase(loremIpsum, "lorem", true, true, false, 0)]
	[TestCase(loremIpsum, "ipsum.", false, false, false, 118)]
	[TestCase(neverForget, "never", true, false, false, 17)]
	[TestCase(neverForget, "never", false, false, false, 22)]
	[TestCase(neverForget, ".", false, false, false, -1)]
	[TestCase(neverForget, /* language=regex */ ".", false, false, true, 1)]
	[TestCase(loremIpsum,  /* language=regex */ "[thiago]{3,}", true, true, true, 58)]
	public void SubstringIndexFinderTest(string input, string reference, bool isBefore, bool ignoreCase, bool useRegex, int expected)
	{
		Assert.AreEqual(expected, new SubstringIndexFinder(reference, isBefore, ignoreCase, useRegex).FindIn(input));
		Assert.Pass();
	}

	[Test]
	[TestCase("new document.txt", 12)]
	[TestCase("savefile", -1)]
	[TestCase("area.effect", 4)]
	[TestCase("using .NET right now", -1)]
	[TestCase("using .NET_right-now", 6)]
	public void FileExtensionIndexFinderTest1(string fileName, int expected)
	{
		Assert.AreEqual(expected, new FileExtensionIndex().FindIn(fileName));
		Assert.Pass();
	}
}