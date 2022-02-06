using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Core.Actions;
using FileRenamer.Core.Indices;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class ChangeCaseActionEditor : UserControl, IActionEditor
{
	#region Fields

	private readonly DataTemplate emptyDataTemplate = new();

	private const string convertRangeTemplateKey = "ConvertRangeTemplate";
	private readonly DataTemplate convertRangeTemplate;

	private const string convertOccurencesDataTemplateKey = "ConvertOccurencesDataTemplate";
	private readonly DataTemplate convertOccurencesDataTemplate;

	#endregion


	#region Properties

	public ChangeCaseActionData Data { get; } = new();

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
			typeof(ChangeCaseActionData),
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
			typeof(ChangeCaseActionEditor),
			new PropertyMetadata(true));
	#endregion IsValid DependencyProperty

	#endregion


	public ChangeCaseActionEditor()
	{
		//
		InitializeComponent();

		//
		if (Resources.TryGetValue(convertRangeTemplateKey, out object o))
			convertRangeTemplate = o as DataTemplate;

		if (Resources.TryGetValue(convertOccurencesDataTemplateKey, out o))
			convertOccurencesDataTemplate = o as DataTemplate;

		UpdateExtraDataTemplate();

		//
		Data.PropertyChanged += Data_PropertyChanged;
	}


	public RenameActionBase GetRenameAction()
	{
		return Data.ExecutionScope switch
		{
			ExecutionScope.WholeInput => new ToCaseAction(new BeginningIndex(), new EndIndex(), Data.TextCase),
			ExecutionScope.Range => new ToCaseAction(Data.StartIndexData.GetIIndex(), Data.EndIndexData.GetIIndex(), Data.TextCase),
			ExecutionScope.Occurrences => throw new NotImplementedException(),
			_ => throw new NotImplementedException(),
		};
	}

	private void UpdateExtraDataTemplate()
	{
		ExtraDataTemplate = Data.ExecutionScope switch
		{
			ExecutionScope.WholeInput => emptyDataTemplate,
			ExecutionScope.Range => convertRangeTemplate,
			ExecutionScope.Occurrences => convertOccurencesDataTemplate,
			_ => emptyDataTemplate,
		};
	}


	private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(Data.HasErrors):
				IsValid = !Data.HasErrors;
				break;

			case nameof(Data.ExecutionScope):
				UpdateExtraDataTemplate();
				break;
		}
	}
}