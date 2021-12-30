using NUnit.Framework;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core_Tester;

public class FindIndexTests
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
	public void BeginingIndexFinderTest()
	{
		if (testStrings == null)
		{
			Assert.Inconclusive($"This test could not be ran because the {nameof(testStrings)} variable is null.");
			return;
		}

		BeginingIndexFinder indexFinder = new();

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
	public void SubstringIndexFinderTest1()
	{
		Assert.AreEqual(19, new SubstringIndexFinder("fox", false).FindIn(quickBrownFox));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest2()
	{
		Assert.AreEqual(40, new SubstringIndexFinder("dog", true).FindIn(quickBrownFox));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest3()
	{
		Assert.AreEqual(29, new SubstringIndexFinder("quick", true).FindIn(fiveBoxingWizards));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest4()
	{
		Assert.AreEqual(34, new SubstringIndexFinder("quick", false).FindIn(fiveBoxingWizards));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest5()
	{
		Assert.AreEqual(2, new SubstringIndexFinder("r", true).FindIn(loremIpsum));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest6()
	{
		Assert.AreEqual(0, new SubstringIndexFinder("Lorem", true).FindIn(loremIpsum));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest7()
	{
		Assert.AreEqual(118, new SubstringIndexFinder("ipsum.", false).FindIn(loremIpsum));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest8()
	{
		Assert.AreEqual(-1, new SubstringIndexFinder("lorem", true).FindIn(loremIpsum));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest9()
	{
		Assert.AreEqual(-1, new SubstringIndexFinder("lorem", false).FindIn(loremIpsum));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest10()
	{
		Assert.AreEqual(17, new SubstringIndexFinder("never", true).FindIn(neverForget));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest11()
	{
		Assert.AreEqual(22, new SubstringIndexFinder("never", false).FindIn(neverForget));
		Assert.Pass();
	}

	[Test]
	public void SubstringIndexFinderTest12()
	{
		Assert.AreEqual(-1, new SubstringIndexFinder(".", false).FindIn(neverForget));
		Assert.Pass();
	}
}