using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

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

	public ChangeCaseActionEditor(ChangeRangeCaseAction action)
	{
		Data = new(action);

		InitializeComponent();
		Initialize();
	}

	public ChangeCaseActionEditor(ChangeStringCaseAction action)
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


	public RenameFileJob GetRenameAction()
	{
		return Data.GetRenameAction();
	}
}