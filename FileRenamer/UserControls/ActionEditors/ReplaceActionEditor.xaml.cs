using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

[ObservableObject]
public sealed partial class ReplaceActionEditor : IActionEditor
{
	public string DialogTitle => "Replace";

	[ObservableProperty]
	public bool _isValid = true;

	#region Data

	public ReplaceActionData Data { get; }

	private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Data.HasErrors))
			Validate();
	}

	#endregion


	public ReplaceActionEditor()
	{
		Data = new();
		Initialize();
	}

	public ReplaceActionEditor(ReplaceAction action)
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


	public RenameActionBase GetRenameAction() => Data.GetRenameAction();

	private void Validate()
	{
		IsValid = !Data.HasErrors;
	}
}