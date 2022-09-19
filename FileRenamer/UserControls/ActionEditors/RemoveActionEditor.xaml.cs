using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ActionEditors;

[ObservableObject]
public sealed partial class RemoveActionEditor : IActionEditor
{
	public string DialogTitle => "Remove";

	[ObservableProperty]
	public bool _isValid = true;

	#region Data

	public RemoveActionData Data { get; }

	private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Data.HasErrors))
			Validate();
	}


	#endregion


	#region Constructors

	public RemoveActionEditor()
	{
		Data = new();
		InitializeComponent();
		Validate();
		Data.PropertyChanged += Data_PropertyChanged;
	}

	public RemoveActionEditor(RemoveAction action)
	{
		Data = new(action);
		InitializeComponent();
		Validate();
		Data.PropertyChanged += Data_PropertyChanged;
	}

	#endregion


	public RenameFileJob GetRenameAction()
	{
		return Data.RangeType switch
		{
			TextRangeType.Count => new RemoveAction(Data.StartIndexData.GetIIndex(), Data.Count),
			TextRangeType.Range => new RemoveAction(Data.StartIndexData.GetIIndex(), Data.EndIndexData.GetIIndex()),
			_ => throw new NotImplementedException(),
		};
	}

	private void Validate()
	{
		IsValid = !Data.HasErrors;
	}
}