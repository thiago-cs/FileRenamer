﻿namespace FileRenamer.UserControls.InputControls;

public sealed class RemoveActionData : System.ComponentModel.BindableBase
{
	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	private RemovalType _actionType = RemovalType.FixedLength;
	public RemovalType ActionType
	{
		get => _actionType;
		set
		{
			if (SetProperty(ref _actionType, value))
				Validate();
		}
	}

	public IndexFinderEditorData StartIndexData { get; } = new();

	public IndexFinderEditorData EndIndexData { get; } = new();

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

	private int _length;
	public int Length
	{
		get => _length;
		set
		{
			if (SetProperty(ref _length, value))
				Validate();
		}
	}

	private string _lengthError;
	public string LengthError { get => _lengthError; set => SetProperty(ref _lengthError, value); }


	public RemoveActionData()
	{
		StartIndexData.PropertyChanged += IndexData_PropertyChanged;
		EndIndexData.PropertyChanged += IndexData_PropertyChanged;
	}


	private void Validate()
	{
		string lengthError = null;
		string endIndexError = null;

		switch (ActionType)
		{
			case RemovalType.FixedLength:
				switch (StartIndexData.IndexType)
				{
					case IndexFinderType.Beginning:
						if (Length <= 0)
							lengthError = "Enter a positive number.";
						break;

					case IndexFinderType.End:
						if (0 <= Length)
							lengthError = "Enter a negative number.";
						break;

					case IndexFinderType.Position:
						if (Length + StartIndexData.IndexPosition < 0)
							lengthError = $"Enter a number greater than {-StartIndexData.IndexPosition}.";
						break;

					case IndexFinderType.FileExtension:
					case IndexFinderType.Before:
					case IndexFinderType.After:
					case IndexFinderType.None:
						break;
					default:
						break;
				}

				if (lengthError == null && Length == 0)
					lengthError = "Enter a number other than zero.";

				break;

			case RemovalType.EndIndex:
				endIndexError = (StartIndexData.IndexType, EndIndexData.IndexType) switch
				{
					(_, IndexFinderType.Beginning) or
					(IndexFinderType.End, _) or
					(IndexFinderType.FileExtension, IndexFinderType.FileExtension) => "Enter valid index types.",
					(IndexFinderType.Position, IndexFinderType.Position) => EndIndexData.IndexPosition <= StartIndexData.IndexPosition ? "Enter valid index positions." : null,
					_ => null,
				};
				break;

			default:
				break;
		}

		//
		LengthError = lengthError;
		EndIndexError = endIndexError;
		UpdateHasErrors();
	}

	private void UpdateHasErrors()
	{
		HasErrors = LengthError != null || EndIndexError != null || StartIndexData.HasErrors || EndIndexData.HasErrors;
	}


	private void IndexData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(IndexFinderEditorData.IndexType):
			case nameof(IndexFinderEditorData.IndexPosition):
				Validate();
				break;

			case nameof(IndexFinderEditorData.HasErrors):
				UpdateHasErrors();
				break;
		}
	}
}