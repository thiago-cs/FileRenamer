using System.Xml;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs;

/// <summary>
/// Represents a collection of actions to be executed in order on a string.
/// </summary>
public sealed class JobCollection : System.Collections.ObjectModel.ObservableCollection<JobItem>, Models.IDeepCopyable<JobCollection>, IXmlSerializableAsync
{
	#region Constructors

	public JobCollection()
	{ }

	public JobCollection(IEnumerable<JobItem> collection)
		: base(collection)
	{ }

	#endregion


	/// <summary>
	/// Executes the actions in this collection, in order, on the specified input.
	/// </summary>
	public void Run(JobTarget target, JobContext context)
	{
		foreach (JobItem job in this)
			if (job.IsEnabled)
				job.Run(target, context);
	}

	public void Reset()
	{
		// todo: implement Reset method.
	}

	public JobCollection DeepCopy()
	{
		return new(this.Select(item => item.DeepCopy()));
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		foreach (JobItem job in this)
			await job.WriteXmlAsync(writer).ConfigureAwait(false);
	}

	public static async Task<JobCollection> ReadXmlAsync(XmlReader reader)
	{
		JobCollection jobs = new();

		while (reader.NodeType != XmlNodeType.EndElement)
		{
			JobItem item = await reader.ReadJobItemAsync().ConfigureAwait(false);
			jobs.Add(item);
		}

		return jobs;
	}

	#endregion
}