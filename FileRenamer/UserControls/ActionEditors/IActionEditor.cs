using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public interface IActionEditor
{
    public string DialogTitle { get; }

    public bool IsValid { get; }

    public RenameFileJob GetRenameAction();
}