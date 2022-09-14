using System.Xml;
using CommunityToolkit.Mvvm.ComponentModel;


namespace FileRenamer.Core.Jobs.FileActions;

public abstract partial class RenameActionBase : ObservableObject, IFileAction, Models.IDeepCopyable<RenameActionBase>
{
	[ObservableProperty]
	private bool _isEnabled = true;

	[ObservableProperty]
	private string _description = "";


	public abstract void Run(JobTarget target, JobContext context);

	public abstract void UpdateDescription();

	public abstract RenameActionBase DeepCopy();


	#region XML serialization

	public abstract Task WriteXmlAsync(XmlWriter writer);

	//public static abstract Task<RenameActionBase> ReadXmlAsync(XmlReader reader);

	#endregion


#if DEBUG
	protected string GetDebuggerDisplay()
	{
		return Description;
	}
#endif
}