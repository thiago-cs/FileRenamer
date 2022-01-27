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

	private IndexFinderType _indexType = IndexFinderType.None;
	public IndexFinderType IndexType
	{
		get => _indexType;
		set
		{
			if (_indexType == value)
				return;

			_indexType = value;
			UpdateExtraDataTemplate();
		}
	}

	public int Number { get; set; }

	public TextType TextType { get; set; }

	public string Text { get; set; }

	public bool IgnoreCase { get; set; }

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
		DataContext = this;

		InitializeComponent();

		if (Resources.TryGetValue(numberInputDataTemplateKey, out object o))
			numberInputDataTemplate = o as DataTemplate;

		if (Resources.TryGetValue(textInputDataTemplateKey, out o))
			textInputDataTemplate = o as DataTemplate;
	}


	private void UpdateExtraDataTemplate()
	{
		ExtraDataTemplate = IndexType switch
		{
			IndexFinderType.Position => numberInputDataTemplate,
			IndexFinderType.After or IndexFinderType.Before => textInputDataTemplate,
			IndexFinderType.None or IndexFinderType.Beginning or IndexFinderType.End or IndexFinderType.FileExtension or _ => emptyDataTemplate,
		};
	}

	public IIndexFinder GetIndexFinder()
	{
		return IndexType switch
		{
			IndexFinderType.None => null,
			IndexFinderType.Beginning => new BeginningIndexFinder(),
			IndexFinderType.End => new EndIndexFinder(),
			IndexFinderType.FileExtension => new FileExtensionIndexFinder(),
			IndexFinderType.Position => new FixedIndexFinder(Number),
			IndexFinderType.Before => new SubstringIndexFinder(Text, true, IgnoreCase, TextType == TextType.Regex),
			IndexFinderType.After => new SubstringIndexFinder(Text, true, IgnoreCase, TextType == TextType.Regex),
			_ => throw new NotImplementedException(),
		};
	}
}