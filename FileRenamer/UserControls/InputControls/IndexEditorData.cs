using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Indices;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class IndexEditorData : ObservableValidator
{
	#region Index Type

	[ObservableProperty]
	private IndexTypeEntry _indexType;

	partial void OnIndexTypeChanged(IndexTypeEntry value)
	{
		ValidateProperty(SearchTextData, nameof(SearchTextData));
	}

	#endregion

	#region Fixed index

	[ObservableProperty]
	private int _indexPosition;

	#endregion

	#region After | Before

	[CustomValidation(typeof(IndexEditorData), nameof(ValidateSearchTextData))]
	public SearchTextData SearchTextData { get; }

	private void SearchTextData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		ValidateProperty(SearchTextData, nameof(SearchTextData));
	}

	#endregion


	#region Constructors

	public IndexEditorData()
	{
		SearchTextData = new();
		IndexType = IndexTypeEntry.FromIndexType(InputControls.IndexType.Beginning);

		Initialize();
	}

	public IndexEditorData(IIndex index)
	{
		SearchTextData = new();

		switch (index)
		{
			case BeginningIndex:
				IndexType = IndexTypeEntry.FromIndexType(InputControls.IndexType.Beginning);
				break;

			case EndIndex:
				IndexType = IndexTypeEntry.FromIndexType(InputControls.IndexType.End);
				break;

			case FileExtensionIndex:
				IndexType = IndexTypeEntry.FromIndexType(InputControls.IndexType.FileExtension);
				break;

			case FixedIndex index1:
				IndexType = IndexTypeEntry.FromIndexType(InputControls.IndexType.Position);
				IndexPosition = index1.Index;
				break;

			case SubstringIndex index1:
				IndexType = index1.Before ? IndexTypeEntry.FromIndexType(InputControls.IndexType.Before) : IndexTypeEntry.FromIndexType(InputControls.IndexType.After);
				SearchTextData.IgnoreCase = index1.IgnoreCase;
				SearchTextData.Text = index1.Value;
				SearchTextData.TextType = index1.UseRegex ? TextType.Regex : TextType.Text;
				break;

			case null:
				break;

			default:
				throw new System.NotImplementedException();
		};

		Initialize();
	}

	private void Initialize()
	{
		//Validate();

		SearchTextData.PropertyChanged += SearchTextData_PropertyChanged;
	}

	#endregion


	public IIndex GetIIndex()
	{
		return HasErrors
			? null
			: IndexType.Type switch
			{
				InputControls.IndexType.Beginning => new BeginningIndex(),
				InputControls.IndexType.End => new EndIndex(),
				InputControls.IndexType.FileExtension => new FileExtensionIndex(),
				InputControls.IndexType.Position => new FixedIndex(IndexPosition),
				InputControls.IndexType.Before => new SubstringIndex(SearchTextData.Text, before: true, SearchTextData.IgnoreCase, SearchTextData.TextType == TextType.Regex),
				InputControls.IndexType.After => new SubstringIndex(SearchTextData.Text, before: false, SearchTextData.IgnoreCase, SearchTextData.TextType == TextType.Regex),
				_ => throw new System.NotImplementedException($"Unknown {nameof(Core.Indices.IIndex)} type '{IndexType}'."),
			};
	}


	#region Validation

	public static ValidationResult ValidateSearchTextData(SearchTextData _searchTextData, ValidationContext context)
	{
		IndexEditorData instance = context.ObjectInstance as IndexEditorData;

		return (instance.IndexType.Type is InputControls.IndexType.Before or InputControls.IndexType.After && instance.SearchTextData.HasErrors)
			 ? new(nameof(SearchTextData))
			 : ValidationResult.Success;
	}

	#endregion

	public static RangeValidationResult ValidateIndicesRange(IndexEditorData start, IndexEditorData end)
	{
		return (start.IndexType.Type, end.IndexType.Type) switch
		{
			(_, InputControls.IndexType.Beginning) or
			(InputControls.IndexType.End, _) or
			(InputControls.IndexType.FileExtension, InputControls.IndexType.FileExtension) => RangeValidationResult.InvalidIndexType,
			(InputControls.IndexType.Position, InputControls.IndexType.Position) when end.IndexPosition <= start.IndexPosition => RangeValidationResult.InvalidIndexPosition,
			_ => RangeValidationResult.Ok,
		};
	}
}