using System;
using Microsoft.UI.Xaml;
using FileRenamer.Core.Indices;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class IndexFinderEditor
{
	#region Fields

	internal static readonly IndexFinderTypeEntry[] IndexFinderTypes = new IndexFinderTypeEntry[]
	{
		new(IndexFinderType.Beginning, "at", "the beginning"),
		new(IndexFinderType.End, "at", "the end"),
		new(IndexFinderType.FileExtension, null, "before extension"),
		new(IndexFinderType.Position, "at", "position"),
		new(IndexFinderType.Before, null, "before"),
		new(IndexFinderType.After, null, "after"),
	};

	internal static readonly TextType[] TextTypes = Enum.GetValues<TextType>();

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
			IndexTypeSelector.DisplayMemberPath = value ? nameof(IndexFinderTypeEntry.DescriptionWithPreposition) : nameof(IndexFinderTypeEntry.DescriptionWithoutPreposition);
		}
	}

	public IndexFinderEditorData Data { get; } = new();

	#region ExtraDataTemplate DependencyProperty
	public DataTemplate ExtraDataTemplate
	{
		get => (DataTemplate)GetValue(ExtraDataTemplateProperty);
		set => SetValue(ExtraDataTemplateProperty, value);
	}

	// Using a DependencyProperty as the backing store for ExtraDataTemplate.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty ExtraDataTemplateProperty =
		DependencyProperty.Register(
			nameof(ExtraDataTemplate),
			typeof(DataTemplate),
			typeof(IndexFinderEditor),
			new PropertyMetadata(null));
	#endregion ExtraDataTemplate DependencyProperty

	#endregion


	public IndexFinderEditor()
	{
		//
		InitializeComponent();

		//
		if (Resources.TryGetValue(numberInputDataTemplateKey, out object o))
			numberInputDataTemplate = o as DataTemplate;

		if (Resources.TryGetValue(textInputDataTemplateKey, out o))
			textInputDataTemplate = o as DataTemplate;

		UpdateExtraDataTemplate();

		//
		Data.PropertyChanged += Data_PropertyChanged;
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
			IndexFinderType.Position => numberInputDataTemplate,
			IndexFinderType.After or IndexFinderType.Before => textInputDataTemplate,
			IndexFinderType.None or IndexFinderType.Beginning or IndexFinderType.End or IndexFinderType.FileExtension or _ => emptyDataTemplate,
		};
	}

	public IIndexFinder GetIndexFinder()
	{
		return Data.IndexType switch
		{
			IndexFinderType.None => null,
			IndexFinderType.Beginning => new BeginningIndexFinder(),
			IndexFinderType.End => new EndIndexFinder(),
			IndexFinderType.FileExtension => new FileExtensionIndexFinder(),
			IndexFinderType.Position => new FixedIndexFinder(Data.IndexPosition),
			IndexFinderType.Before => new SubstringIndexFinder(Data.Text, true, Data.IgnoreCase, Data.TextType == TextType.Regex),
			IndexFinderType.After => new SubstringIndexFinder(Data.Text, true, Data.IgnoreCase, Data.TextType == TextType.Regex),
			_ => throw new NotImplementedException(),
		};
	}
}