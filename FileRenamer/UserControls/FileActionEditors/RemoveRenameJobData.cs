using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Helpers;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class RemoveRenameJobData : ObservableValidator, IJobEditorData
{
	#region Constants

	private const string INVALID_INDEX_TYPE_ERROR_MESSAGE = "Enter valid index types.";
	private const string INVALID_INDEX_POSITION_ERROR_MESSAGE = "Enter valid index positions.";

	public static readonly TextRangeType[] RangeTypes = System.Enum.GetValues<TextRangeType>();

	#endregion

	#region RangeType

	[ObservableProperty]
	private TextRangeType _rangeType = TextRangeType.Count;

	partial void OnRangeTypeChanged(TextRangeType value)
	{
		switch (value)
		{
			case TextRangeType.Count:
				ClearErrors(nameof(EndIndexData));
				ValidateProperty(Count, nameof(Count));
				break;

			case TextRangeType.Range:
				ClearErrors(nameof(Count));
				ValidateProperty(EndIndexData, nameof(EndIndexData));
				break;
		}
	}

	#endregion

	#region StartIndex

	[NoErrors]
	public IndexEditorData StartIndexData { get; }

	private void StartIndexData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(IndexEditorData.IndexType):
			case nameof(IndexEditorData.IndexPosition):
				ValidateProperty(EndIndexData, nameof(EndIndexData));
				break;

			case nameof(IndexEditorData.HasErrors):
				ValidateProperty(sender, nameof(StartIndexData));
				break;
		}
	}

	#endregion

	#region EndIndex

	[NoErrors]
	[CustomValidation(typeof(RemoveRenameJobData), nameof(ValidateIndices))]
	public IndexEditorData EndIndexData { get; }

	[ObservableProperty]
	private string _endIndexErrorMessage;

	private void EndIndexData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(IndexEditorData.IndexType):
			case nameof(IndexEditorData.IndexPosition):
			case nameof(IndexEditorData.HasErrors):
				ValidateProperty(EndIndexData, nameof(EndIndexData));
				break;
		}
	}

	#endregion

	#region Count

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(RemoveRenameJobData), nameof(ValidateCount))]
	private int _count = 1;

	[ObservableProperty]
	private string _countErrorMessage;

	#endregion


	#region Constructors

	public RemoveRenameJobData()
	{
		StartIndexData = new();
		EndIndexData = new();
		RangeType = TextRangeType.Count;
		Initialize();
	}

	public RemoveRenameJobData(RemoveAction action)
	{
		StartIndexData = new(action.StartIndex);
		EndIndexData = new(action.EndIndex);
		RangeType = action.EndIndex == null ? TextRangeType.Count : TextRangeType.Range;
		Initialize();
	}

	private void Initialize()
	{
		StartIndexData.PropertyChanged += StartIndexData_PropertyChanged;
		EndIndexData.PropertyChanged += EndIndexData_PropertyChanged;
	}

	#endregion


	public Core.Jobs.JobItem GetJobItem()
	{
		return RangeType switch
		{
			TextRangeType.Count => new RemoveAction(StartIndexData.GetIIndex(), Count),
			TextRangeType.Range => new RemoveAction(StartIndexData.GetIIndex(), EndIndexData.GetIIndex()),
			_ => throw new System.NotImplementedException(),
		};
	}


	#region Validation

	public static ValidationResult ValidateIndices(IndexEditorData indexData, ValidationContext context)
	{
		RemoveRenameJobData instance = context.ObjectInstance as RemoveRenameJobData;

		if (instance.RangeType != TextRangeType.Range)
			return ValidationResult.Success;

		/*
		string errorMessage = (instance.StartIndexData.IndexType.Type, instance.EndIndexData.IndexType.Type) switch
		{
			(_, IndexType.Beginning) or
			(IndexType.End, _) or
			(IndexType.FileExtension, IndexType.FileExtension) => INVALID_INDEX_TYPE_ERROR_MESSAGE,
			(IndexType.Position, IndexType.Position) when instance.EndIndexData.IndexPosition <= instance.StartIndexData.IndexPosition => INVALID_INDEX_POSITION_ERROR_MESSAGE,
			_ => null,
		};
		/*/
		string errorMessage = IndexEditorData.ValidateIndicesRange(instance.StartIndexData, instance.EndIndexData) switch
		{
			Core.Indices.RangeValidationResult.Ok => null,
			Core.Indices.RangeValidationResult.InvalidIndexType => INVALID_INDEX_TYPE_ERROR_MESSAGE,
			Core.Indices.RangeValidationResult.InvalidIndexPosition => INVALID_INDEX_POSITION_ERROR_MESSAGE,
			_ => throw new System.NotImplementedException(),
		};
		//*/

		instance.EndIndexErrorMessage = errorMessage;

		return errorMessage == null ? ValidationResult.Success : new(errorMessage);
	}

	public static ValidationResult ValidateCount(int count, ValidationContext context)
	{
		RemoveRenameJobData instance = context.ObjectInstance as RemoveRenameJobData;
		string errorMessage = null;

		if (count == 0)
			errorMessage = "Enter a number other than zero.";

		switch (instance.StartIndexData.IndexType.Type)
		{
			case IndexType.Beginning:
				if (count <= 0)
					errorMessage = "Enter a positive number.";
				break;

			case IndexType.End:
				if (0 <= count)
					errorMessage = "Enter a negative number.";
				break;

			case IndexType.Position:
				if (instance.StartIndexData.IndexPosition < 0)
				{
					if (/*0 < Length &&*/ -instance.StartIndexData.IndexPosition < count)
						errorMessage = $"Enter a number less than or equal to {-instance.StartIndexData.IndexPosition}.";
				}
				else
				{
					if (count < -instance.StartIndexData.IndexPosition)
						errorMessage = $"Enter a number greater than {-instance.StartIndexData.IndexPosition}.";
				}

				break;

			case IndexType.FileExtension:
			case IndexType.Before:
			case IndexType.After:
			default:
				break;
		}

		instance.CountErrorMessage = errorMessage;

		return errorMessage == null ? ValidationResult.Success : new(errorMessage);
	}

	#endregion
}