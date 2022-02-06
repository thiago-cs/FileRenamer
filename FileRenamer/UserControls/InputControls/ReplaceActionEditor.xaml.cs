using System.ComponentModel;
using Microsoft.UI.Xaml;
using FileRenamer.Core.Actions;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class ReplaceActionEditor : IActionEditor
{
	#region Properties

	public ReplaceActionData Data { get; } = new();

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
			typeof(ReplaceActionEditor),
			new PropertyMetadata(null));
	#endregion ExtraDataTemplate DependencyProperty

	#region RangeInputVisibility DependencyProperty
	public Visibility RangeInputVisibility
	{
		get => (Visibility)GetValue(RangeInputVisibilityProperty);
		set => SetValue(RangeInputVisibilityProperty, value);
	}

	public static readonly DependencyProperty RangeInputVisibilityProperty =
		DependencyProperty.Register(
			nameof(RangeInputVisibility),
			typeof(Visibility),
			typeof(ReplaceActionEditor),
			new PropertyMetadata(Visibility.Collapsed));
	#endregion RangeInputVisibility DependencyProperty

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
			typeof(ReplaceActionEditor),
			new PropertyMetadata(false));
	#endregion IsValid DependencyProperty

	#endregion


	public ReplaceActionEditor()
	{
		InitializeComponent();
		Data.PropertyChanged += Data_PropertyChanged;
	}


	public RenameActionBase GetRenameAction()
	{
		return Data.ExecutionScope switch
		{
			ExecutionScope.WholeInput => new ReplaceAction(Data.OldString.Text, Data.NewString, Data.OldString.IgnoreCase, Data.OldString.TextType == TextType.Regex),
			ExecutionScope.Range => new ReplaceAction(Data.StartIndexData.GetIIndex(), Data.EndIndexData.GetIIndex(), Data.OldString.Text, Data.NewString, Data.OldString.IgnoreCase, Data.OldString.TextType == TextType.Regex),
			_ => null,
		};
	}


	private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(Data.ExecutionScope):
				RangeInputVisibility = Data.ExecutionScope == ExecutionScope.Range ? Visibility.Visible : Visibility.Collapsed;
				break;

			case nameof(Data.HasErrors):
				IsValid = !Data.HasErrors;
				break;
		}
	}
}