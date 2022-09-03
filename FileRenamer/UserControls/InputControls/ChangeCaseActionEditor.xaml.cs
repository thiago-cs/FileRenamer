using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.InputControls;

[INotifyPropertyChanged]
public sealed partial class ChangeCaseActionEditor : UserControl, IActionEditor
{
	#region Properties

	public string DialogTitle => "Change case";

	[ObservableProperty]
	private bool _isValid = true;

	public ChangeCaseActionData Data { get; }

	private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Data.HasErrors))
			IsValid = !Data.HasErrors;
	}

	#endregion


	#region Contructors

	public ChangeCaseActionEditor()
	{
		Data = new();

		InitializeComponent();
		Initialize();
	}

	public ChangeCaseActionEditor(ToCaseAction action)
	{
		Data = new(action);

		InitializeComponent();
		Initialize();
	}

	private void Initialize()
	{
		Data.PropertyChanged += Data_PropertyChanged;
	}

	#endregion


	public RenameActionBase GetRenameAction()
	{
		return Data.ExecutionScope switch
		{
			ExecutionScope.FileName => new ToCaseAction(new BeginningIndex(), new FileExtensionIndex(), Data.TextCase),
			ExecutionScope.FileExtension => new ToCaseAction(new FileExtensionIndex(), new EndIndex(), Data.TextCase),
			ExecutionScope.WholeInput => new ToCaseAction(new BeginningIndex(), new EndIndex(), Data.TextCase),
			ExecutionScope.CustomRange => new ToCaseAction(Data.RangeData.StartIndexData.GetIIndex(), Data.RangeData.EndIndexData.GetIIndex(), Data.TextCase),
			// TODO: come back here once ChangeStringCaseAction is implemented.
			//ExecutionScope.Occurrences => new ChangeStringCaseAction(Data.SearchText.Text, Data.SearchText.TextType == TextType.Regex, Data.SearchText.IgnoreCase, Data.TextCase),
			_ => throw new NotImplementedException(),
		};
	}
}