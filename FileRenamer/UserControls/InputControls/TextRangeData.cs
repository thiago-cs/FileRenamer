﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Indices;
using FileRenamer.Helpers;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class TextRangeData : ObservableValidator
{
	#region Constants

	private const string INVALID_INDEX_TYPE_ERROR_MESSAGE = "Enter valid index types.";
	private const string INVALID_INDEX_POSITION_ERROR_MESSAGE = "Enter valid index positions.";

	#endregion


	#region StartIndex

	[NoErrors]
	public IndexEditorData StartIndexData { get; }

	private void IndexData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(IndexEditorData.IndexType):
			case nameof(IndexEditorData.IndexPosition):
				ValidateEndIndex();
				break;
		}
	}

	#endregion

	#region EndIndex

	[NoErrors]
	[CustomValidation(typeof(TextRangeData), nameof(ValidateEndIndex))]
	public IndexEditorData EndIndexData { get; }

	[ObservableProperty]
	private string _endIndexErrorMessage;

	#endregion


	#region Constructors

	public TextRangeData()
	{
		StartIndexData = new();
		EndIndexData = new();
		Initialize();
	}

	public TextRangeData(IIndex startIndex, IIndex endIndex)
	{
		StartIndexData = new(startIndex);
		EndIndexData = new(endIndex);
		Initialize();
	}

	private void Initialize()
	{
		ValidateAllProperties();

		StartIndexData.PropertyChanged += IndexData_PropertyChanged;
		EndIndexData.PropertyChanged += IndexData_PropertyChanged;
	}

	#endregion


	#region Validation

	private void ValidateEndIndex()
	{
		ValidateProperty(EndIndexData, nameof(EndIndexData));
	}

	public static ValidationResult ValidateEndIndex(IndexEditorData endIndexData, ValidationContext context)
	{
		TextRangeData instance = context.ObjectInstance as TextRangeData;

		string errorMessage = IndexEditorData.ValidateIndicesRange(instance.StartIndexData, instance.EndIndexData) switch
		{
			RangeValidationResult.Ok => null,
			RangeValidationResult.InvalidIndexType => INVALID_INDEX_TYPE_ERROR_MESSAGE,
			RangeValidationResult.InvalidIndexPosition => INVALID_INDEX_POSITION_ERROR_MESSAGE,
			_ => throw new System.NotImplementedException(),
		};

		instance.EndIndexErrorMessage = errorMessage;

		return errorMessage == null ? ValidationResult.Success : new(errorMessage);
	}

	#endregion
}