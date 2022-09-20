using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs;

/// <summary>
/// Represents a collection of actions to be executed in order on a string.
/// </summary>
public sealed class JobCollection : System.Collections.ObjectModel.ObservableCollection<JobItem>, Models.IDeepCopyable<JobCollection>, IXmlSerializableAsync
{
	#region Constructors

	public JobCollection()
	{
		CollectionChanged += Base_CollectionChanged;
	}

	public JobCollection(IEnumerable<JobItem> collection)
		: base(collection)
	{
		CollectionChanged += Base_CollectionChanged;
	}

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


	#region Nested job changed notification

	public event PropertyChangedEventHandler? NestedJobChanged;

	private void Base_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		//
		if (e.OldItems != null)
			foreach (JobItem job in e.OldItems)
			{
				job.PropertyChanged -= NestedItem_PropertyChanged;

				if (job is ComplexJobItem complexJob)
					complexJob.Jobs.NestedJobChanged -= NestedItem_PropertyChanged;
			}

		//
		if (e.NewItems != null)
			foreach (JobItem job in e.NewItems)
			{
				job.PropertyChanged += NestedItem_PropertyChanged;

				if (job is ComplexJobItem complexJob)
					complexJob.Jobs.NestedJobChanged += NestedItem_PropertyChanged;
			}
	}

	private void NestedItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		NestedJobChanged?.Invoke(this, e);
	}

	#endregion


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