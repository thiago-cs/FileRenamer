using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

public static class RenameActionExensions
{
	public static string Run(this RenameActionBase action, string input)
	{
		return action.Run(input, NoContext);
	}

	public static string Run(this RenameActionBase action, string input, JobContext context)
	{
		JobTarget target = new(new FileMock(input), 0);
		action.Run(target, context);

		return target.NewFileName;
	}
}