using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_InsertCounterAction
{
	[Test]
	[TestCase("new document ().txt", 14, 1, 1, new string[] { "new document (1).txt", "new document (2).txt", "new document (3).txt" })]
	[TestCase("big file.part", 13, 0, 3, new string[] { "big file.part000", "big file.part001", "big file.part002", "big file.part003", })]
	public void Test(string input, int insertPosition, int startValue, int width, string[] expected)
	{
		InsertCounterAction insertCounterAction = new(new FixedIndexFinder(insertPosition), startValue, width);

		for (int i = 0; i < expected.Length; i++)
			Assert.AreEqual(expected[i], insertCounterAction.Run(input));

		Assert.Pass();
	}
}