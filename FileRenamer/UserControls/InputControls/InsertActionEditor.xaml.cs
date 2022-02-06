using System;
using Microsoft.UI.Xaml;
using FileRenamer.Core.Actions;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class InsertActionEditor : IActionEditor
{
	#region Fields

	public static readonly StringSourceType[] stringSourceTypes = Enum.GetValues<StringSourceType>();

	private readonly DataTemplate emptyDataTemplate = new();

	private const string textInputDataTemplateKey = "TextInputDataTemplate";
	private readonly DataTemplate textInputDataTemplate;

	private const string counterInputDataTemplateKey = "CounterInputDataTemplate";
	private readonly DataTemplate counterInputDataTemplate;

	#endregion


	#region Properties

	public InsertActionData Data { get; } = new();

	#region ExtraDataTemplate DependencyProperty
	public DataTemplate ExtraDataTemplate
	{
		get => (DataTemplate)GetValue(ExtraDataTemplateProperty);
		set => SetValue(ExtraDataTemplateProperty, value);
	}

	public static readonly DependencyProperty ExtraDataTemplateProperty =
		DependencyProperty.Register(
			nameof(ExtraDataTemplate),
			typeof(DataTemplate),
			typeof(IndexEditor),
			new PropertyMetadata(null));
	#endregion ExtraDataTemplate DependencyProperty

	#region IsValid DependencyProperty
	public bool IsValid
	{
		get => (bool)GetValue(IsValidProperty);
		set => SetValue(IsValidProperty, value);
	}

	public static readonly DependencyProperty IsValidProperty =
		DependencyProperty.Register(
			nameof(IsValid),
			typeof(bool),
			typeof(InsertActionEditor),
			new PropertyMetadata(false));
	#endregion IsValid DependencyProperty

	#endregion


	public InsertActionEditor()
	{
		//
		InitializeComponent();

		//
		if (Resources.TryGetValue(textInputDataTemplateKey, out object o))
			textInputDataTemplate = o as DataTemplate;

		if (Resources.TryGetValue(counterInputDataTemplateKey, out o))
			counterInputDataTemplate = o as DataTemplate;

		ExtraDataTemplate = textInputDataTemplate;

		//
		Data.PropertyChanged += Data_PropertyChanged;
	}


	public RenameActionBase GetRenameAction()
	{
		if (!IsValid)
		{
			// Oops!
			return null;
		}

		Core.Indices.IIndex indexFinder = Data.IndexData.GetIndexFinder();

		switch (Data.StringType)
		{
			case StringSourceType.Text:
				return new InsertAction(indexFinder, Data.Text);

			case StringSourceType.Counter:
				return new InsertCounterAction(indexFinder, Data.InitialValue, Data.PaddedLength/*, Data.PaddingChar[0]*/);

			default:
				// Oops!
				return null;
		}
	}

	private void UpdateExtraDataTemplate()
	{
		ExtraDataTemplate = Data.StringType switch
		{
			StringSourceType.Text => textInputDataTemplate,
			StringSourceType.Counter => counterInputDataTemplate,
			//StringSourceType.Mp3Tag => throw new NotImplementedException(),
			//StringSourceType.ExifTag => throw new NotImplementedException(),
			_ => emptyDataTemplate,
		};
	}


	private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(Data.StringType):
				UpdateExtraDataTemplate();
				break;

			case nameof(Data.HasErrors):
				IsValid = !Data.HasErrors;
				break;
		}
	}
}