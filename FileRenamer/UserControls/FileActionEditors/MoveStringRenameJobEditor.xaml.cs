using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class MoveStringRenameJobEditor : IJobEditor<MoveStringRenameJobData>
{
	public string DialogTitle => "Move";

	public MoveStringRenameJobData Data { get; }


	public MoveStringRenameJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public MoveStringRenameJobEditor(MoveStringAction action)
	{
		Data = new(action);
		InitializeComponent();
	}
}