using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

public class RemoveActionTests
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
		Assert.AreEqual("",
						Compute(quickBrownFox, new BeginingIndexFinder(), new EndIndexFinder()));
		Assert.Pass();
	}

	[Test]
	public void Test2()
	{
		Assert.AreEqual("The five boxing wizards",
						Compute(fiveBoxingWizards, new SubstringIndexFinder("wizards", false), new EndIndexFinder()));
		Assert.Pass();
	}

	[Test]
	public void Test3()
	{
		Assert.AreEqual("Lorem ipsum dolor sit amet. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.",
						Compute(loremIpsum, new SubstringIndexFinder(", ", true), new SubstringIndexFinder(".", true)));
		Assert.Pass();
	}

	[Test]
	public void Test4()
	{
		Assert.AreEqual("never forget kindnesses",
						Compute(neverForget, new BeginingIndexFinder(), new SubstringIndexFinder(", ", false)));
		Assert.Pass();
	}

	[Test]
	public void Test5()
	{
		Assert.AreEqual(quickBrownFox,
						Compute(quickBrownFox, new SubstringIndexFinder("dog", true), new SubstringIndexFinder("fox", true)));
		Assert.Pass();
	}

	[Test]
	public void Test6()
	{
		Assert.AreEqual(loremIpsum,
						Compute(loremIpsum, new SubstringIndexFinder("Hello", true), new SubstringIndexFinder(".", true)));
		Assert.Pass();
	}

	[Test]
	public void Test7()
	{
		Assert.AreEqual(loremIpsum,
						Compute(loremIpsum, new SubstringIndexFinder("o", true), new SubstringIndexFinder("World!", true)));
		Assert.Pass();
	}


	private static string Compute(string input, IIndexFinder startIndexFinder, IIndexFinder endIndexFinder)
	{
		return new RemoveAction(startIndexFinder, endIndexFinder).Run(input);
	}
}