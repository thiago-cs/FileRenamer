using System.Xml;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs;

/// <summary>
/// Represents a collection of actions to be executed in order on a string.
/// </summary>
public sealed class JobCollection : System.Collections.ObjectModel.ObservableCollection<IJobItem>, IXmlSerializableAsync
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


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		foreach (IJobItem job in this)
			await job.WriteXmlAsync(writer).ConfigureAwait(false);
	}

	public static async Task<JobCollection> ReadXmlAsync(XmlReader reader)
	{
		JobCollection jobs = new();

		while (reader.NodeType != XmlNodeType.EndElement)
		{
			IJobItem item = await reader.ReadJobItemAsync().ConfigureAwait(false);
			jobs.Add(item);
		}

		return jobs;
	}

	#endregion
}