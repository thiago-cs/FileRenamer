using NUnit.Framework;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Actions;
using static FileRenamer.Core_Tester.Resources;


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


	#region Description tests

	[Test]
	[TestCase(0, 00, 1, @"insert a 1-char counter starting from 0 at the beginning")]
	[TestCase(6, 00, 1, @"insert a 1-char counter starting from 0 at the end")]
	[TestCase(1, 01, 2, @"insert a 2-char counter starting from 1 after char. #3")]
	[TestCase(2, 01, 2, @"insert a 2-char counter starting from 1 before ""sunset""")]
	[TestCase(3, 10, 3, @"insert a 3-char counter starting from 10 after ""dark""")]
	[TestCase(4, 20, 3, @"insert a 3-char counter starting from 20 after the expression ""(Hi|Hello) kitty""")]
	[TestCase(5, 99, 3, @"insert a 3-char counter starting from 99 before file's extension")]
	public void TestDescriptionBeginning(int indexFinderIndex, int startValue, int minWidth, string expected)
	{
		Assert.AreEqual(expected, new InsertCounterAction(finders[indexFinderIndex], startValue, minWidth).Description);
	}

	#endregion
}