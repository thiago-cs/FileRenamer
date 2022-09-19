using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class ReplaceRenameJobEditor : IJobEditor
{
	public string DialogTitle => "Replace";

	public ReplaceRenameJobData Data { get; }
	IJobEditorData IJobEditor.Data => Data;


	public ReplaceRenameJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public ReplaceRenameJobEditor(ReplaceAction action)
	{
		Data = new(action);
		InitializeComponent();
	}
}