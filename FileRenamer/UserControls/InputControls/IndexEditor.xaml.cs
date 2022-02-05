using Microsoft.UI.Xaml;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class IndexEditor
{
	#region Fields

	internal static readonly IndexTypeEntry[] IndexFinderTypes = new IndexTypeEntry[]
	{
		new(IndexType.Beginning, "at", "the beginning"),
		new(IndexType.End, "at", "the end"),
		new(IndexType.FileExtension, null, "before extension"),
		new(IndexType.Position, "at", "position"),
		new(IndexType.Before, null, "before"),
		new(IndexType.After, null, "after"),
	};

	private readonly DataTemplate emptyDataTemplate = new();

	private const string numberInputDataTemplateKey = "NumberInputDataTemplate";
	private readonly DataTemplate numberInputDataTemplate;

	private const string textInputDataTemplateKey = "TextInputDataTemplate";
	private readonly DataTemplate textInputDataTemplate;

	#endregion


	#region Properties

	private bool _includePreposionInDescriptions;
	public bool IncludePreposionInDescriptions
	{
		get => _includePreposionInDescriptions;
		set
		{
			_includePreposionInDescriptions = value;
			IndexTypeSelector.DisplayMemberPath = value ? nameof(IndexTypeEntry.DescriptionWithPreposition) : nameof(IndexTypeEntry.DescriptionWithoutPreposition);
		}
	}

	#region Data DependencyProperty
	public IndexEditorData Data
	{
		get => (IndexEditorData)GetValue(DataProperty);
		set => SetValue(DataProperty, value);
	}

	public static readonly DependencyProperty DataProperty =
		DependencyProperty.Register(
			nameof(Data),
			typeof(IndexEditorData),
			typeof(IndexEditor),
			new PropertyMetadata(null, OnDataChanged));

	private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not IndexEditor @this)
			return;

		if (e.OldValue is IndexEditorData oldData)
			oldData.PropertyChanged -= @this.Data_PropertyChanged;

		if (e.NewValue is IndexEditorData newData)
			newData.PropertyChanged += @this.Data_PropertyChanged;
	}
	#endregion Data DependencyProperty

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

	#endregion


	public IndexEditor()
	{
		//
		InitializeComponent();

		//
		if (Resources.TryGetValue(numberInputDataTemplateKey, out object o))
			numberInputDataTemplate = o as DataTemplate;

		if (Resources.TryGetValue(textInputDataTemplateKey, out o))
			textInputDataTemplate = o as DataTemplate;

		ExtraDataTemplate = emptyDataTemplate;
	}


	private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Data.IndexType))
			UpdateExtraDataTemplate();
	}

	private void UpdateExtraDataTemplate()
	{
		ExtraDataTemplate = Data.IndexType switch
		{
			IndexType.Position => numberInputDataTemplate,
			IndexType.After or IndexType.Before => textInputDataTemplate,
			IndexType.None or IndexType.Beginning or IndexType.End or IndexType.FileExtension or _ => emptyDataTemplate,
		};
	}
}