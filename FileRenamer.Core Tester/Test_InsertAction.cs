using NUnit.Framework;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_InsertAction
{
	#region Insertion tests

	[Test]
	[TestCase(quickBrownFox, "Maybe ")]
	[TestCase("", "Empty")]
	public void AddToBeginning(string input, string addend)
	{
		JobTarget target = new(new FileMock(input), 0);
		IFileAction action = new InsertAction(new BeginningIndex(), (StringValueSource)addend);
		action.Run(target, NoContext);

		Assert.AreEqual(addend + input, target.NewFileName);
	}

	[Test]
	[TestCase(neverForget, ".")]
	public void AddToEnd(string input, string addend)
	{
		JobTarget target = new(new FileMock(input), 0);
		IFileAction action = new InsertAction(new EndIndex(), (StringValueSource)addend);
		action.Run(target, NoContext);

		Assert.AreEqual(input + addend, target.NewFileName);
	}

	[Test]
	[TestCase(fiveBoxingWizards, "wizards", false, " do not", "The five boxing wizards do not jump quickly.")]
	[TestCase(loremIpsum, "Etiam ", false, "m", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam mat sem consectetur, egestas velit vitae, lacinia ipsum.")]
	[TestCase(quickBrownFox, "dog", true, "old ", "the quick brown fox jumps over the lazy old dog.")]
	public void InsertInTheMiddle(string input, string reference, bool isBefore, string addend, string expected)
	{
		JobTarget target = new(new FileMock(input), 0);
		IFileAction action = new InsertAction(new SubstringIndex(reference, isBefore, false, false), (StringValueSource)addend);
		action.Run(target, NoContext);

		Assert.AreEqual(expected, target.NewFileName);
	}

	#endregion


	#region Description tests

	[Test]
	[TestCase(0, "this", @"insert ""this"" at the beginning")]
	[TestCase(6, "that", @"insert ""that"" at the end")]
	[TestCase(1, "third", @"insert ""third"" after the 3rd character")]
	[TestCase(2, "Jesse", @"insert ""Jesse"" before ""sunset""")]
	[TestCase(3, "mint", @"insert ""mint"" after ""dark""")]
	[TestCase(4, "!", @"insert ""!"" after the expression ""(Hi|Hello) kitty""")]
	[TestCase(5, ".part", @"insert "".part"" before file's extension")]
	public void TestDescriptionBeginning(int indexFinderIndex, string value, string expected)
	{
		Assert.AreEqual(expected, new InsertAction(finders[indexFinderIndex], (StringValueSource)value).Description);
	}

	#endregion
}