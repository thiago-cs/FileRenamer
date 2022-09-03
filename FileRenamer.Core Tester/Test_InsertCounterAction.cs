using System.Linq;
using NUnit.Framework;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;
using FileRenamer.Core.ValueSources.NumberFormatters;
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
		JobTarget[] targets = expected.Select((_, i) => new JobTarget(new FileMock(input), i)).ToArray();
		CounterValueSource cvs = new() { InitialValue = startValue, Formatter = new PaddedNumberFormatter() { MinWidth = width } };
		JobCollection jobs = new()
		{
			new InsertAction(new FixedIndex(insertPosition), cvs),
		};
		JobContext context = new(jobs, targets);

		for (int i = 0; i < expected.Length; i++)
		{
			new InsertAction(new FixedIndex(insertPosition), cvs).Run(targets[i], context);

			Assert.AreEqual(expected[i], targets[i].NewFileName);
		}
	}


	#region Description tests

	[Test]
	[TestCase(0, 00, 1, @"insert a counter (0, 1, 2, …) at the beginning")]
	[TestCase(6, 00, 1, @"insert a counter (0, 1, 2, …) at the end")]
	[TestCase(1, 01, 2, @"insert a counter (01, 02, 03, …) after the 3rd character")]
	[TestCase(2, 01, 2, @"insert a counter (01, 02, 03, …) before ""sunset""")]
	[TestCase(3, 10, 3, @"insert a counter (010, 011, 012, …) after ""dark""")]
	[TestCase(4, 20, 3, @"insert a counter (020, 021, 022, …) after the expression ""(Hi|Hello) kitty""")]
	[TestCase(5, 99, 3, @"insert a counter (099, 100, 101, …) before file's extension")]
	public void TestDescriptionForPaddedCardinalNumbers(int indexFinderIndex, int startValue, int minWidth, string expected)
	{
		CounterValueSource cvs = new() { InitialValue = startValue, Formatter = new PaddedNumberFormatter() { MinWidth = minWidth } };
		InsertAction insertAction = new(finders[indexFinderIndex], cvs);

		Assert.AreEqual(expected, insertAction.Description);
	}

	[Test]
	[TestCase(0, 01, true, @"insert a counter using Roman numerals (I, II, III, …) at the beginning")]
	[TestCase(6, 04, false, @"insert a counter using Roman numerals (iv, v, vi, …) at the end")]
	[TestCase(3, 10, false, @"insert a counter using Roman numerals (x, xi, xii, …) after ""dark""")]
	[TestCase(4, 20, true, @"insert a counter using Roman numerals (XX, XXI, XXII, …) after the expression ""(Hi|Hello) kitty""")]
	[TestCase(5, 99, false, @"insert a counter using Roman numerals (xcix, c, ci, …) before file's extension")]
	public void TestDescriptionForRomanNumbers(int indexFinderIndex, int startValue, bool useUppercase, string expected)
	{
		CounterValueSource cvs = new() { InitialValue = startValue, Formatter = new RomanNumberFormatter() { UseUppercase = useUppercase } };
		InsertAction insertAction = new(finders[indexFinderIndex], cvs);

		Assert.AreEqual(expected, insertAction.Description);
	}

	#endregion
}