using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class ChangeCaseRenameJobEditor : IJobEditor<ChangeCaseRenameJobData>
{
	#region Properties

	public string DialogTitle => "Change case";

	public ChangeCaseRenameJobData Data { get; }

	#endregion


	#region Contructors

	public ChangeCaseRenameJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public ChangeCaseRenameJobEditor(ChangeRangeCaseAction action)
	{
		Data = new(action);
		InitializeComponent();
	}

	public ChangeCaseRenameJobEditor(ChangeStringCaseAction action)
	{
		Data = new(action);
		InitializeComponent();
	}

	#endregion
}