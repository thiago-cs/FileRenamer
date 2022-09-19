using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

[ObservableObject]
public sealed partial class MoveStringRenameJobEditor : IJobEditor
{
	public string DialogTitle => "Move";

	[ObservableProperty]
	public bool _isValid = true;

	#region Data

	public MoveStringRenameJobData Data { get; }

	private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Data.HasErrors))
			Validate();
	}

	#endregion


	public MoveStringRenameJobEditor()
	{
		Data = new();
		Initialize();
	}

	public MoveStringRenameJobEditor(MoveStringAction action)
	{
		Data = new(action);
		Initialize();
	}

	private void Initialize()
	{
		InitializeComponent();
		Validate();
		Data.PropertyChanged += Data_PropertyChanged;
	}


	public JobItem GetRenameAction() => Data.GetRenameAction();

	private void Validate()
	{
		IsValid = !Data.HasErrors;
	}
}