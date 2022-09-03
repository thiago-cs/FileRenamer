using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.ViewModels.ValueSources;

internal sealed partial class StringValueSourceViewModel : ObservableValidator, IValueSourceViewModel
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
	[Required(ErrorMessage = emptyTextErrorMessage)]
	[MaxLength(100)]
	[NotifyPropertyChangedFor(nameof(TextErrorMessage))]
	private string _text;

	public string TextErrorMessage => GetErrors(nameof(Text))?.FirstOrDefault()?.ErrorMessage;


	public IValueSource ValueSource => new StringValueSource() { Value = Text };


	public StringValueSourceViewModel() : this(new())
	{ }

	public StringValueSourceViewModel(StringValueSource valueSource)
	{
		Text = valueSource.Value;
	}
}