using FileRenamer.Core.Jobs;


namespace FileRenamer.UserControls.ActionEditors;

public interface IJobEditor
{
	public string DialogTitle { get; }

	public bool IsValid { get; }

	public JobItem GetRenameAction();
}