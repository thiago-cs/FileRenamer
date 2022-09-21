using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class InsertRenameJobEditor : IJobEditor<InsertRenameJobData>
{
	#region Properties

	public string DialogTitle => "Insert";

	public InsertRenameJobData Data { get; }

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