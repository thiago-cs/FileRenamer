using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class SearchTextData : ObservableValidator
{
	private const string ERROR_MESSAGE_TEXT_EMPTY = "Enter the pattern to search for.";
	private const string ERROR_MESSAGE_INVALID_REGEX = "Enter a valid pattern.";
	private readonly Helpers.DelayedAction delayedValidation;


	#region TextType

	internal static readonly TextType[] TextTypes = Enum.GetValues<TextType>();

	[ObservableProperty]
	private TextType _textType;

	partial void OnTextTypeChanged(TextType value)
	{
		_ = InvokeTextValidationAsync();
	}

	#endregion

	#region Text and text error

	[ObservableProperty]
	//[NotifyDataErrorInfo]
	[CustomValidation(typeof(SearchTextData), nameof(ValidateText))]
	private string _text;

	partial void OnTextChanged(string value)
	{
		_ = InvokeTextValidationAsync();
	}

	[ObservableProperty]
	private string _textError;

	#endregion

	private System.Text.RegularExpressions.Regex Regex;

	public bool IgnoreCase { get; set; }


	public SearchTextData()
	{
		delayedValidation = new(() => ValidateProperty(Text, nameof(Text)));
		ValidateAllProperties();
	}


	#region Validation

	private async Task InvokeTextValidationAsync()
	{
		await delayedValidation.InvokeAsync(TextType == TextType.Text ? 200 : 1000);
	}

	public static ValidationResult ValidateText(string text, ValidationContext context)
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