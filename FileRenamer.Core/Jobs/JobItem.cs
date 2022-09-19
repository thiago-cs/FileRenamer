using System.Xml;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Models;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs;

public abstract partial class JobItem : ObservableObject, IDeepCopyable<JobItem>, IXmlSerializableAsync
{
	[ObservableProperty]
	private bool _isEnabled = true;

	[ObservableProperty]
	private string _description = "";


	public abstract void Run(JobTarget target, JobContext context);

	public abstract JobItem DeepCopy();

	public abstract Task WriteXmlAsync(XmlWriter writer);


#if DEBUG
	protected string GetDebuggerDisplay()
	{
		return Description;
	}
#endif
}