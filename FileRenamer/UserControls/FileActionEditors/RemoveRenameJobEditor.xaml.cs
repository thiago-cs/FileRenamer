using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class RemoveRenameJobEditor : IJobEditor<RemoveRenameJobData>
{
	public string DialogTitle => "Remove";

	public RemoveRenameJobData Data { get; }


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