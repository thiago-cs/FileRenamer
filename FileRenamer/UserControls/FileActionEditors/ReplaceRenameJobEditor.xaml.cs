using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class ReplaceRenameJobEditor : IJobEditor<ReplaceRenameJobData>
{
	public string DialogTitle => "Replace";

	public ReplaceRenameJobData Data { get; }


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