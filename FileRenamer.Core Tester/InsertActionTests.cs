using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

public class InsertActionTests
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
		Assert.AreEqual("Maybe " + quickBrownFox, 
						Compute(new BeginingIndexFinder(), "Maybe ", quickBrownFox));
		Assert.Pass();
	}

	[Test]
	public void Test2()
	{
		Assert.AreEqual("The five boxing wizards do not jump quickly.", 
						Compute(new SubstringIndexFinder("wizards", false), " do not", fiveBoxingWizards));
		Assert.Pass();
	}

	[Test]
	public void Test3()
	{
		Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam mat sem consectetur, egestas velit vitae, lacinia ipsum.",
						Compute(new SubstringIndexFinder("Etiam ", false), "m", loremIpsum));
		Assert.Pass();
	}

	[Test]
	public void Test4()
	{
		Assert.AreEqual(neverForget + ".",
						Compute(new EndIndexFinder(), ".", neverForget));
		Assert.Pass();
	}

	[Test]
	public void Test5()
	{
		Assert.AreEqual("the quick brown fox jumps over the lazy old dog.",
						Compute(new SubstringIndexFinder("dog", true), "old ", quickBrownFox));
		Assert.Pass();
	}


	private static string Compute(IIndexFinder insertIndexFinder, string value, string input)
	{
		return new InsertAction(insertIndexFinder, value).Run(input);
	}
}