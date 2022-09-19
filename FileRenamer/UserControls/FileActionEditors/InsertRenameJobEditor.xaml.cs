using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class InsertRenameJobEditor : IJobEditor
{
	#region Properties

	public string DialogTitle => "Insert";

	public InsertRenameJobData Data { get; }
	IJobEditorData IJobEditor.Data => Data;

	#endregion


	#region Constructors

	public InsertRenameJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public InsertRenameJobEditor(InsertAction insertAction)
	{
		Data = new(insertAction);
		InitializeComponent();
	}

	#endregion
}