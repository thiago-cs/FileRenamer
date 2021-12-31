using NUnit.Framework;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core_Tester;

public class Test_FindIndex
{
	#region Fields

	private const string quickBrownFox = "the quick brown fox jumps over the lazy dog.";
	private const string fiveBoxingWizards = "The five boxing wizards jump quickly.";
	private const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.";
	private const string neverForget = "Forget injuries, never forget kindnesses";

	private string[]? testStrings;

	#endregion


	[SetUp]
	public void Setup()
	{
		testStrings = new[]
			{
				string.Empty,
				loremIpsum,
				quickBrownFox,
				fiveBoxingWizards,
				neverForget,
			};
	}

	[Test]
	public void BeginningIndexFinderTest()
	{
		if (testStrings == null)
		{
			Assert.Inconclusive($"This test could not be ran because the {nameof(testStrings)} variable is null.");
			return;
		}

		BeginningIndexFinder indexFinder = new();

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

		EndIndexFinder indexFinder = new();

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
			Assert.AreEqual(r, new FixedIndexFinder(r).FindIn(testStrings[i]));
		}

		Assert.Pass();
	}

	[Test]
	[TestCase(quickBrownFox, "fox", false, 19)]
	[TestCase(quickBrownFox, "dog", true, 40)]
	[TestCase(fiveBoxingWizards, "quick", true, 29)]
	[TestCase(fiveBoxingWizards, "quick", false, 34)]
	[TestCase(loremIpsum, "r", true, 2)]
	[TestCase(loremIpsum, "Lorem", true, 0)]
	[TestCase(loremIpsum, "ipsum.", false, 118)]
	[TestCase(loremIpsum, "lorem", true, -1)]
	[TestCase(loremIpsum, "lorem", false, -1)]
	[TestCase(neverForget, "never", true, 17)]
	[TestCase(neverForget, "never", false, 22)]
	[TestCase(neverForget, ".", false, -1)]
	public void SubstringIndexFinderTest(string input, string reference, bool isBefore, int expected)
	{
		Assert.AreEqual(expected, new SubstringIndexFinder(reference, isBefore).FindIn(input));
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
		Assert.AreEqual(expected, new FileExtensionIndexFinder().FindIn(fileName));
		Assert.Pass();
	}
}