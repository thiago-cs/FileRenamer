using FileRenamer.Core.Indices;


namespace FileRenamer.UserControls.InputControls;

public sealed class IndexEditorData : System.ComponentModel.BindableBase
{
	#region Basic

	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	private IndexType _indexType;
	public IndexType IndexType
	{
		get => _indexType;
		set
		{
			if (SetProperty(ref _indexType, value))
				Validate();
		}
	}

	private string _indexTypeError;
	public string IndexTypeError { get => _indexTypeError; private set => SetProperty(ref _indexTypeError, value); }

	#endregion

	#region Position

	private int _indexPosition;
	public int IndexPosition { get => _indexPosition; set => SetProperty(ref _indexPosition, value); }

	#endregion

	#region After | Before

	public SearchTextData SearchTextData { get; } = new();

	#endregion


	public IndexEditorData()
	{
		Validate();
		SearchTextData.PropertyChanged += SearchTextData_PropertyChanged;
	}


	public IIndexFinder GetIndexFinder()
	{
		return HasErrors
			? null
			: IndexType switch
			{
				IndexType.None => null,
				IndexType.Beginning => new BeginningIndexFinder(),
				IndexType.End => new EndIndexFinder(),
				IndexType.FileExtension => new FileExtensionIndexFinder(),
				IndexType.Position => new FixedIndexFinder(IndexPosition),
				IndexType.Before => new SubstringIndexFinder(SearchTextData.Text, before: true, SearchTextData.IgnoreCase, SearchTextData.TextType == TextType.Regex),
				IndexType.After => new SubstringIndexFinder(SearchTextData.Text, before: false, SearchTextData.IgnoreCase, SearchTextData.TextType == TextType.Regex),
				_ => throw new System.NotImplementedException($"Unknown {nameof(IIndexFinder)} type '{IndexType}'."),
			};
	}

	#region Validation

	private void Validate()
	{
		// 1. 
		string indexTypeError = null;

		if (IndexType == IndexType.None)
			indexTypeError = "Select an index type.";

		// 2. 
		IndexTypeError = indexTypeError;
		UpdateHasErrors();
	}

	private void UpdateHasErrors()
	{
		HasErrors = IndexTypeError != null || (IndexType is IndexType.Before or IndexType.After && SearchTextData.HasErrors);
	}

	private static bool IsRegexPatternValid(string pattern)
	{
		try
		{
			_ = new System.Text.RegularExpressions.Regex(pattern);
			return true;
		}
		catch
		{
			return false;
		}
	}

	#endregion


	private void SearchTextData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(SearchTextData.HasErrors))
			UpdateHasErrors();
	}
}