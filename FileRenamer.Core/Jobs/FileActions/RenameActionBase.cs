namespace FileRenamer.Core.Jobs.FileActions;

public abstract class RenameActionBase : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, IFileAction, Models.IDeepCopyable<RenameActionBase>
{
	private bool _isEnabled = true;
	public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

	private string _description = "";
	public string Description { get => _description; set => SetProperty(ref _description, value); }


	public abstract void Run(JobTarget target, JobContext context);

	public abstract void UpdateDescription();

	public abstract RenameActionBase DeepCopy();


#if DEBUG
	protected string GetDebuggerDisplay()
	{
		return Description;
	}
#endif
}