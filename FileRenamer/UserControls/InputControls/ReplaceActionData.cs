using System;
using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public sealed class ReplaceActionData : BindableBase
{
	public static readonly ExecutionScope[] executionScopeTypes = { ExecutionScope.WholeInput, ExecutionScope.Range };


	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	public SearchTextData OldString { get; } = new();

	public string NewString { get; set; }

	private ExecutionScope _executionScope;
	public ExecutionScope ExecutionScope
	{
		get => _executionScope;
		set
		{
			if (SetProperty(ref _executionScope, value))
				Validate();
		}
	}

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


	public ReplaceActionData()
	{
		Validate();
		StartIndexData.PropertyChanged += IndexData_PropertyChanged;
		EndIndexData.PropertyChanged += IndexData_PropertyChanged;
		OldString.PropertyChanged += OldString_PropertyChanged;
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
		HasErrors = OldString.HasErrors || 
					(ExecutionScope == ExecutionScope.Range && (StartIndexData.HasErrors || EndIndexData.HasErrors || EndIndexError != null));
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

	private void OldString_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(BindableBase.HasErrors))
			UpdateHasErrors();
	}
}