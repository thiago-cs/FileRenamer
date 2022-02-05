namespace FileRenamer.UserControls.InputControls;

public sealed class InsertActionData : System.ComponentModel.BindableBase
{
	#region Basic

	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	private StringSourceType _stringType;
	public StringSourceType StringType
	{
		get => _stringType;
		set
		{
			if (SetProperty(ref _stringType, value))
				Validate();
		}
	}

	private string _stringTypeError;
	public string StringTypeError { get => _stringTypeError; private set => SetProperty(ref _stringTypeError, value); }

	public IndexEditorData IndexData { get; } = new();

	private static System.Collections.Generic.HashSet<char> _invalidPathChars;
	private static System.Collections.Generic.HashSet<char> InvalidPathChars => _invalidPathChars ??= new(System.IO.Path.GetInvalidFileNameChars());

	#endregion

	#region Text

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

	#endregion

	#region Counter

	public int InitialValue { get; set; } = 1;

	public int Increment { get; set; } = 1;

	public int PaddedLength { get; set; } = 1;

	private string _paddingChar = "0";
	public string PaddingChar
	{
		get => _paddingChar;
		set
		{
			if (SetProperty(ref _paddingChar, value))
				Validate();
		}
	}

	private string _paddingCharError;
	public string PaddingCharError { get => _paddingCharError; private set => SetProperty(ref _paddingCharError, value); }

	#endregion


	public InsertActionData()
	{
		Validate();
		IndexData.PropertyChanged += IndexData_PropertyChanged;
	}


	private void Validate()
	{
		string stringTypeError = null;
		string textError = null;
		string paddingCharError = null;

		switch (StringType)
		{
			case StringSourceType.Text:
				if (string.IsNullOrEmpty(Text))
					textError = "Enter the text to insert.";
				break;

			case StringSourceType.Counter:
				if (string.IsNullOrEmpty(PaddingChar))
				{
					paddingCharError = "Enter the padding char.";
				}
				else
				{
					char c = PaddingChar[0];

					if (InvalidPathChars.Contains(c) || (c != ' ' && !char.IsDigit(c) && !char.IsLetter(c) && !char.IsPunctuation(c)))
						paddingCharError = "Enter a valid char.";
				}
				break;

			default:
				stringTypeError = "Chose a different type.";
				break;
		}

		//
		StringTypeError = stringTypeError;
		TextError = textError;
		PaddingCharError = paddingCharError;
		UpdateHasErrors();
	}

	private void UpdateHasErrors()
	{
		HasErrors = StringTypeError != null || TextError != null || PaddingCharError != null || IndexData.HasErrors;
	}


	private void IndexData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(IndexEditorData.HasErrors))
			UpdateHasErrors();
	}
}