using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Helpers;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class SearchTextData : ObservableValidator
{
	private const string ERROR_MESSAGE_TEXT_EMPTY = "Enter the pattern to search for.";
	private const string ERROR_MESSAGE_INVALID_REGEX = "Enter a valid pattern.";
	private readonly Func<Task> debouncedValidateTextAsync;


	#region TextType

	internal static readonly TextType[] TextTypes = Enum.GetValues<TextType>();

	[ObservableProperty]
	private TextType _textType;

	partial void OnTextTypeChanged(TextType value)
	{
		_ = debouncedValidateTextAsync();
	}

	#endregion

	#region Text and text error

	[ObservableProperty]
	//[NotifyDataErrorInfo]
	[CustomValidation(typeof(SearchTextData), nameof(ValidateText))]
	private string _text;

	partial void OnTextChanged(string value)
	{
		_ = debouncedValidateTextAsync();
	}

	[ObservableProperty]
	private string _textError;

	#endregion

	private System.Text.RegularExpressions.Regex Regex;

	public bool IgnoreCase { get; set; }


	public SearchTextData()
	{
		Action action = () => ValidateProperty(Text, nameof(Text));
		debouncedValidateTextAsync = action.DebounceAsync(200);
		ValidateAllProperties();
	}


	#region Validation

	public static ValidationResult ValidateText(string _text, ValidationContext context)
	{
		// 1. 
		SearchTextData instance = context.ObjectInstance as SearchTextData;
		string textError = null;

		if (instance.TextType == TextType.Regex)
		{
			if (!instance.IsRegexPatternValid())
				textError = ERROR_MESSAGE_INVALID_REGEX;
		}
		else
		{
			if (string.IsNullOrEmpty(instance.Text))
				textError = ERROR_MESSAGE_TEXT_EMPTY;
		}

		// 2. 
		instance.TextError = textError;
		return textError == null ? ValidationResult.Success : new(textError);
	}

	private bool IsRegexPatternValid()
	{
		Regex = null;

		try
		{
			if (!string.IsNullOrEmpty(Text))
				Regex = new System.Text.RegularExpressions.Regex(Text);
		}
		catch
		{ }

		return Regex != null;
	}

	#endregion
}