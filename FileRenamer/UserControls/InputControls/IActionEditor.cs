using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.InputControls;

public interface IActionEditor
{
	public string DialogTitle { get; }

	public bool IsValid { get; }

	public RenameActionBase GetRenameAction();
}