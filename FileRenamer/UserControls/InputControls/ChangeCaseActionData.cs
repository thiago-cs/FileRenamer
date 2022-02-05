using System;
using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public sealed class ChangeCaseActionData : BindableBase
{
	public static readonly ExecutionScope[] executionScopeTypes = { ExecutionScope.WholeInput, ExecutionScope.Range, ExecutionScope.Occurrences };
	public static readonly Core.Extensions.TextCasing[] textCases = Enum.GetValues<Core.Extensions.TextCasing>();


	#region Basic

	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	private ExecutionScope _executionScope = ExecutionScope.WholeInput;
	public ExecutionScope ExecutionScope
	{
		get => _executionScope;
		set
		{
			if (SetProperty(ref _executionScope, value))
				Validate();
		}
	}

	public Core.Extensions.TextCasing TextCase { get; set; } = Core.Extensions.TextCasing.SentenceCase;

	#endregion

	#region Range

	public IndexEditorData StartIndexData { get; } = new();

	public IndexEditorData EndIndexData { get; } = new();

	private string _endIndexError;
	public string EndIndexError
	{
		get => _endIndexError;
		set
		{
			if (SetProperty(ref _endIndexError, value))
				Validate();
		}
	}

	#endregion

	#region Occurences of a text/pattern

	public SearchTextData SearchText { get; } = new();

	#endregion


	public ChangeCaseActionData()
	{
		StartIndexData.PropertyChanged += IndexData_PropertyChanged;
		EndIndexData.PropertyChanged += IndexData_PropertyChanged;
		SearchText.PropertyChanged += SearchText_PropertyChanged;
	}


	#region Validation

	private void Validate()
	{
		//// 1. 
		//string indexTypeError = null;

		//if (IndexType == IndexFinderType.None)
		//	indexTypeError = "Select an index type.";

		//// 2. 
		//IndexTypeError = indexTypeError;
		UpdateHasErrors();
	}

	private void UpdateHasErrors()
	{
		HasErrors = ExecutionScope switch
		{
			ExecutionScope.WholeInput => false,
			ExecutionScope.Range => StartIndexData.HasErrors || EndIndexData.HasErrors || EndIndexError != null,
			ExecutionScope.Occurrences => SearchText.HasErrors,
			_ => throw new NotImplementedException(),
		};
	}

	#endregion


	private void IndexData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(BindableBase.HasErrors):
				UpdateHasErrors();
				break;

			case nameof(IndexEditorData.IndexType):
			case nameof(IndexEditorData.IndexPosition):
				Validate();
				break;
		}
	}

	private void SearchText_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(BindableBase.HasErrors))
			UpdateHasErrors();
	}
}