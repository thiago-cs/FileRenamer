using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.ViewModels.ValueSources;

public sealed partial class StringValueSourceViewModel : ObservableValidator, IValueSourceViewModel
{
	private const string emptyTextErrorMessage = "Enter the text to be inserted.";

	// todo: validate symbols against the list of invalid file name characters.
	//private static System.Collections.Generic.HashSet<char> _invalidPathChars;
	//private static System.Collections.Generic.HashSet<char> InvalidPathChars => _invalidPathChars ??= new(System.IO.Path.GetInvalidFileNameChars());

	//if (cvs.Formatter is PaddedNumberFormatter pnf)
	//	if (string.IsNullOrEmpty(pnf.PaddingChar))
	//	{
	//		paddingCharError = "Enter the padding char.";
	//	}
	//	else
	//	{
	//		char c = PaddingChar[0];

	//		if (InvalidPathChars.Contains(c) || (c != ' ' && !char.IsDigit(c) && !char.IsLetter(c) && !char.IsPunctuation(c)))
	//			paddingCharError = "Enter a valid char.";
	//	}


	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(StringValueSourceViewModel), nameof(ValidateText))]
	[MaxLength(100)]
	[NotifyPropertyChangedFor(nameof(TextErrorMessage))]
	private string _text;

	public string TextErrorMessage => GetErrors(nameof(Text))?.FirstOrDefault()?.ErrorMessage;


	public IValueSource ValueSource => new StringValueSource(Text);


	#region Constructors

	public StringValueSourceViewModel()
		: this(new())
	{ }

	public StringValueSourceViewModel(StringValueSource valueSource)
	{
		Text = valueSource.Value;

		ValidateAllProperties();
	}

	#endregion


	#region Validation

	public static ValidationResult ValidateText(string value, ValidationContext context)
	{
		return string.IsNullOrEmpty(value)
			 ? new(emptyTextErrorMessage)
			 : ValidationResult.Success;
	}
	#endregion
}