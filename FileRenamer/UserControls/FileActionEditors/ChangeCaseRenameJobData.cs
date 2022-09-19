using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Helpers;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class ChangeCaseRenameJobData : ObservableValidator
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

	[NoErrors]
	public TextRangeData RangeData { get; }

	private void RangeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(RangeData, nameof(RangeData));
	}

	#endregion

	#region Occurences of a text/pattern

	[NoErrors]
	public SearchTextData SearchText { get; } = new();

	private void SearchText_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(SearchText, nameof(SearchText));
	}

	#endregion


	#region Constructors

	public ChangeCaseRenameJobData()
	{
		RangeData = new();

		Initialize();
	}

	public ChangeCaseRenameJobData(ChangeRangeCaseAction action)
	{
		RangeData = new(action.StartIndex, action.EndIndex);
		ExecutionScope = ReplaceRenameJobData.GetScopeFromIndices(action.StartIndex, action.EndIndex);
		TextCase = action.TextCase;

		Initialize();
	}

	public ChangeCaseRenameJobData(ChangeStringCaseAction action)
	{
		RangeData = new();
		ExecutionScope = ExecutionScope.Occurrences;
		TextCase = action.TextCase;

		SearchText.Text = action.OldString;
		SearchText.TextType = action.UseRegex ? TextType.Regex : TextType.Text;
		SearchText.IgnoreCase = action.IgnoreCase;

		Initialize();
	}

	private void Initialize()
	{
		RangeData.PropertyChanged += RangeData_PropertyChanged;
		SearchText.PropertyChanged += SearchText_PropertyChanged;
	}

	#endregion


	public RenameFileJob GetRenameAction()
	{
		return ExecutionScope switch
		{
			ExecutionScope.FileName => new ChangeRangeCaseAction(new BeginningIndex(), new FileExtensionIndex(), TextCase),
			ExecutionScope.FileExtension => new ChangeRangeCaseAction(new FileExtensionIndex(), new EndIndex(), TextCase),
			ExecutionScope.WholeInput => new ChangeRangeCaseAction(new BeginningIndex(), new EndIndex(), TextCase),
			ExecutionScope.CustomRange => new ChangeRangeCaseAction(RangeData.StartIndexData.GetIIndex(), RangeData.EndIndexData.GetIIndex(), TextCase),
			ExecutionScope.Occurrences => new ChangeStringCaseAction(SearchText.Text, SearchText.TextType == TextType.Regex, SearchText.IgnoreCase, TextCase),
			_ => throw new NotImplementedException(),
		};
	}
}