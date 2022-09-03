using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.Core.Jobs;

/// <summary>
/// Represents a collection of actions to be executed in order on a string.
/// </summary>
public sealed class JobCollection : System.Collections.ObjectModel.ObservableCollection<IJobItem>
{
	/// <summary>
	/// Executes the actions in this collection, in order, on the specified input.
	/// </summary>
	// <param name="input">A string.</param>
	// <returns>The resulting string after the actions have been executed on it.</returns>
	public void Run(JobTarget target, JobContext context)
	{
		foreach (IJobItem job in this)
			if (job is RenameActionBase action)
			{
				if (action.IsEnabled)
					action.Run(target, context);
			}
	}

	public void Reset()
	{
		// todo: implement Reset method.
	}
}