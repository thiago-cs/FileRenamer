using FileRenamer.Core.Actions;


namespace FileRenamer.UserControls.InputControls;

internal interface IActionEditor
{
	bool IsValid { get; }

	RenameActionBase GetRenameAction();
}