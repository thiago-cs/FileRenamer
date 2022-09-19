namespace FileRenamer.Core.Jobs.FileActions;

public abstract partial class RenameActionBase : ObservableObject, IFileAction, Models.IDeepCopyable<RenameActionBase>
{
	public abstract void UpdateDescription();
}