using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class RemoveRenameJobEditor : IJobEditor
{
	public string DialogTitle => "Remove";

	public RemoveRenameJobData Data { get; }
	IJobEditorData IJobEditor.Data => Data;


	#region Constructors

	public RemoveRenameJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public RemoveRenameJobEditor(RemoveAction action)
	{
		Data = new(action);
		InitializeComponent();
	}

	#endregion
}