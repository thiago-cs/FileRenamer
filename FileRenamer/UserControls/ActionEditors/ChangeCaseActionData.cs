using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class ChangeCaseActionData : ObservableValidator
{
	#region Constants

	public static readonly ExecutionScope[] executionScopeTypes = Enum.GetValues<ExecutionScope>();

	public static readonly Core.Extensions.TextCasing[] textCases = Enum.GetValues<Core.Extensions.TextCasing>();

	#endregion


	#region Scope and Case

	[ObservableProperty]
	private ExecutionScope _executionScope = ExecutionScope.WholeInput;

	partial void OnExecutionScopeChanged(ExecutionScope value)
	{
		ClearErrors();

		switch (ExecutionScope)
		{
			case ExecutionScope.WholeInput:
				break;

			case ExecutionScope.CustomRange:
				ValidateProperty(RangeData, nameof(RangeData));
				break;

			case ExecutionScope.Occurrences:
				ValidateProperty(SearchText, nameof(SearchText));
				break;
		};
	}

	public Core.Extensions.TextCasing TextCase { get; set; } = Core.Extensions.TextCasing.SentenceCase;

	#endregion

	#region Range

	[CustomValidation(typeof(ChangeCaseActionData), nameof(ValidateObservableValidator))]
	public TextRangeData RangeData { get; }

	private void RangeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(RangeData, nameof(RangeData));
	}

	#endregion

	#region Occurences of a text/pattern

	[CustomValidation(typeof(ChangeCaseActionData), nameof(ValidateObservableValidator))]
	public SearchTextData SearchText { get; } = new();

	private void SearchText_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(SearchText, nameof(SearchText));
	}

	#endregion


	#region Constructors

	public ChangeCaseActionData()
	{
		RangeData = new();

		Initialize();
	}

	public ChangeCaseActionData(ChangeRangeCaseAction action)
	{
		RangeData = new(action.StartIndex, action.EndIndex);
		ExecutionScope = ReplaceActionData.GetScopeFromIndices(action.StartIndex, action.EndIndex);
		TextCase = action.TextCase;

		Initialize();
	}

	private void Initialize()
	{
		RangeData.PropertyChanged += RangeData_PropertyChanged;
		SearchText.PropertyChanged += SearchText_PropertyChanged;
	}

	#endregion


	#region Validation

	public static ValidationResult ValidateObservableValidator(INotifyDataErrorInfo notifier, ValidationContext _)
	{
		return notifier.HasErrors
			 ? new("This object has errors.")
			 : ValidationResult.Success;
	}

	#endregion
}