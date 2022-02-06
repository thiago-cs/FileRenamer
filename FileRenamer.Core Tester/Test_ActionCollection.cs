using NUnit.Framework;
using FileRenamer.Core;
using FileRenamer.Core.Actions;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Extensions;


namespace FileRenamer.Core_Tester;

[TestFixture]
public static class Test_ActionCollection
{
	[Test]
	public static void TestRun1()
	{
		const string input = "y2mate.com - Task vs ValueTask When Should I use ValueTask_1080p.MP4";
		const string expected = "Task vs. ValueTask When Should I use ValueTask - Brian Lagunas.mp4";

		ActionCollection actions = new()
		{
			new RemoveAction(new BeginningIndex(), new SubstringIndexFinder(" - ", false, false, false)),
			new InsertAction(new SubstringIndexFinder("vs", false, false, false), "."),
			new ReplaceAction(@"_\d*p", " - Brian Lagunas", false, true),
			new ToCaseAction(new FileExtensionIndex(), new EndIndex(), TextCasing.LowerCase),
		};

		Assert.AreEqual(expected, actions.Run(input));
	}
}