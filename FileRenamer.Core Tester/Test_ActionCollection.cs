using NUnit.Framework;
using FileRenamer.Core.Extensions;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public static class Test_ActionCollection
{
	[Test]
	public static void TestRun1()
	{
		const string input = "y2mate.com - Task vs ValueTask When Should I use ValueTask_1080p.MP4";
		const string expected = "Task vs. ValueTask When Should I use ValueTask - Brian Lagunas.mp4";

		JobCollection actions = new()
		{
			new RemoveAction(new BeginningIndex(), new SubstringIndex(" - ", false, false, false)),
			new InsertAction(new SubstringIndex("vs", false, false, false), (StringValueSource)"."),
			new ReplaceAction(@"_\d*p", " - Brian Lagunas", false, true),
			new ToCaseAction(new FileExtensionIndex(), new EndIndex(), TextCasing.LowerCase),
		};

		JobTarget target = new(new FileMock(input), 0);
		actions.Run(target, Resources.NoContext);

		Assert.AreEqual(expected, target.NewFileName);
	}
}