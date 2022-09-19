using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

[ObservableObject]
public sealed partial class InsertActionEditor : IActionEditor
{
	#region Properties

	public string DialogTitle => "Insert";

	public InsertActionData Data { get; }

	[ObservableProperty]
	// The opposite of HasErrors
	public bool _isValid;

	#endregion


	#region Constructors

	public InsertActionEditor()
	{
		Data = new();
		Initialize();
	}

	public InsertActionEditor(InsertAction insertAction)
	{
		Data = new(insertAction);
		Initialize();
	}

	private void Initialize()
	{
		IsValid = !Data.HasErrors;
		InitializeComponent();

		Data.PropertyChanged += Data_PropertyChanged;
	}

	#endregion


	public RenameFileJob GetRenameAction()
	{
		if (!IsValid)
		{
			// Oops!
			return null;
		}

		return new InsertAction(Data.IndexData.GetIIndex(), Data.ValueSource);
	}


	private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Data.HasErrors))
			IsValid = !Data.HasErrors;
	}
}