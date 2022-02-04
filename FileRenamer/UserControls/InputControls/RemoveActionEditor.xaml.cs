using System;
using Microsoft.UI.Xaml;
using FileRenamer.Core.Actions;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class RemoveActionEditor : IActionEditor
{
	#region Fields

	public static readonly RemovalType[] actionTypes = Enum.GetValues<RemovalType>();

	private readonly DataTemplate emptyDataTemplate = new();

	private const string lengthInputDataTemplateKey = "LengthInputDataTemplate";
	private readonly DataTemplate lengthInputDataTemplate;

	private const string endIndexInputDataTemplateKey = "EndIndexInputDataTemplate";
	private readonly DataTemplate endIndexInputDataTemplate;

	#endregion


	#region Properties

	public RemoveActionData Data { get; } = new();

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
			typeof(RemoveActionEditor),
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
			typeof(RemoveActionEditor),
			new PropertyMetadata(false));
	#endregion IsValid DependencyProperty

	#endregion


	public RemoveActionEditor()
	{
		//
		InitializeComponent();

		//
		if (Resources.TryGetValue(lengthInputDataTemplateKey, out object o))
			lengthInputDataTemplate = o as DataTemplate;

		if (Resources.TryGetValue(endIndexInputDataTemplateKey, out o))
			endIndexInputDataTemplate = o as DataTemplate;

		UpdateExtraDataTemplate();

		//
		Data.PropertyChanged += Data_PropertyChanged;
	}


	public RenameActionBase GetRenameAction()
	{
		return Data.ActionType switch
		{
			RemovalType.FixedLength => new RemoveAction(Data.StartIndexData.GetIndexFinder(), Data.Length),
			RemovalType.EndIndex => new RemoveAction(Data.StartIndexData.GetIndexFinder(), Data.EndIndexData.GetIndexFinder()),
			_ => throw new NotImplementedException(),
		};
	}

	private void UpdateExtraDataTemplate()
	{
		ExtraDataTemplate = Data.ActionType switch
		{
			RemovalType.FixedLength => lengthInputDataTemplate,
			RemovalType.EndIndex => endIndexInputDataTemplate,
			_ => emptyDataTemplate,
		};
	}


	private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(Data.ActionType):
				UpdateExtraDataTemplate();
				break;

			case nameof(Data.HasErrors):
				IsValid = !Data.HasErrors;
				break;
		}
	}
}