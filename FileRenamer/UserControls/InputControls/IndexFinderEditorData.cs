﻿using FileRenamer.Core.Indices;


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

	public SearchTextData SearchTextData { get; } = new();

	#endregion


	public IndexFinderEditorData()
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
				IndexFinderType.None => null,
				IndexFinderType.Beginning => new BeginningIndexFinder(),
				IndexFinderType.End => new EndIndexFinder(),
				IndexFinderType.FileExtension => new FileExtensionIndexFinder(),
				IndexFinderType.Position => new FixedIndexFinder(IndexPosition),
				IndexFinderType.Before => new SubstringIndexFinder(SearchTextData.Text, before: true, SearchTextData.IgnoreCase, SearchTextData.TextType == TextType.Regex),
				IndexFinderType.After => new SubstringIndexFinder(SearchTextData.Text, before: false, SearchTextData.IgnoreCase, SearchTextData.TextType == TextType.Regex),
				_ => throw new System.NotImplementedException($"Unknown {nameof(IIndexFinder)} type '{IndexType}'."),
			};
	}

	#region Validation

	private void Validate()
	{
		// 1. 
		string indexTypeError = null;

		if (IndexType == IndexFinderType.None)
			indexTypeError = "Select an index type.";

		// 2. 
		IndexTypeError = indexTypeError;
		UpdateHasErrors();
	}

	private void UpdateHasErrors()
	{
		HasErrors = IndexTypeError != null || (IndexType is IndexFinderType.Before or IndexFinderType.After && SearchTextData.HasErrors);
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