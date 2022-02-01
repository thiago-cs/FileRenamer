using FileRenamer.Core.Indices;


namespace FileRenamer.UserControls.InputControls;

public sealed class IndexFinderEditorData : System.ComponentModel.BindableBase
{
	#region Basic

	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	private IndexFinderType _indexType;
	public IndexFinderType IndexType
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

	private TextType _textType;
	public TextType TextType
	{
		get => _textType;
		set
		{
			if (SetProperty(ref _textType, value))
				Validate();
		}
	}

	private string _text;
	public string Text
	{
		get => _text;
		set
		{
			if (SetProperty(ref _text, value))
				Validate();
		}
	}

	private string _textError;
	public string TextError { get => _textError; private set => SetProperty(ref _textError, value); }

	public bool IgnoreCase { get; set; }

	#endregion


	public IndexFinderEditorData()
	{
		Validate();
	}


	public IIndexFinder GetIndexFinder()
	{
		return HasErrors
			? null
			: IndexType switch
			{
				IndexFinderType.None => null,
				IndexFinderType.Beginning => new BeginningIndexFinder(),
				IndexFinderType.End => new EndIndexFinder(),
				IndexFinderType.FileExtension => new FileExtensionIndexFinder(),
				IndexFinderType.Position => new FixedIndexFinder(IndexPosition),
				IndexFinderType.Before => new SubstringIndexFinder(Text, true, IgnoreCase, TextType == TextType.Regex),
				IndexFinderType.After => new SubstringIndexFinder(Text, true, IgnoreCase, TextType == TextType.Regex),
				_ => throw new System.NotImplementedException($"Unknown {nameof(IIndexFinder)} type '{IndexType}'."),
			};
	}

	#region Validation

	private void Validate()
	{
		// 1. 
		string indexTypeError = null;
		string textError = null;

		switch (IndexType)
		{
			case IndexFinderType.None:
				indexTypeError = "Select index type.";
				break;

			case IndexFinderType.Before:
			case IndexFinderType.After:
				if (TextType == TextType.Text)
				{
					if (string.IsNullOrEmpty(Text))
						textError = "Enter a text.";
				}
				else if (TextType == TextType.Regex)
				{
					if (string.IsNullOrEmpty(Text))
						textError = "Enter a pattern.";
					else if (!IsRegexPatternValid(Text))
						textError = "Enter a valid pattern.";
				}
				break;

			case IndexFinderType.Beginning:
			case IndexFinderType.End:
			case IndexFinderType.FileExtension:
			case IndexFinderType.Position:
			default:
				break;
		}

		// 2. 
		IndexTypeError = indexTypeError;
		TextError = textError;
		HasErrors = IndexTypeError != null || TextError != null;
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
}