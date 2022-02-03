using System;
using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public sealed class SearchTextData : BindableBase
{
	private readonly Helpers.DelayedAction filteredValidation;

	internal static readonly TextType[] TextTypes = Enum.GetValues<TextType>();


	private bool _hasErrors;
	public new bool HasErrors { get => _hasErrors; private set => SetProperty(ref _hasErrors, value); }

	private TextType _textType;
	public TextType TextType
	{
		get => _textType;
		set
		{
			if (SetProperty(ref _textType, value))
				Validate();
		}
	}

	private string _text;
	public string Text
	{
		get => _text;
		set
		{
			if (SetProperty(ref _text, value))
			{
				HasErrors = true;
				_ = filteredValidation.InvokeAsync(TextType == TextType.Regex ? 1000 : 500);
			}
		}
	}

	private string _textError;
	public string TextError { get => _textError; private set => SetProperty(ref _textError, value); }

	public bool IgnoreCase { get; set; }


	public SearchTextData()
	{
		filteredValidation = new(Validate);
		Validate();
	}


	#region Validation

	private void Validate()
	{
		// 1. 
		string textError = null;

		if (TextType == TextType.Text)
		{
			if (string.IsNullOrEmpty(Text))
				textError = "Enter a text.";
		}
		else if (TextType == TextType.Regex)
		{
			if (string.IsNullOrEmpty(Text))
				textError = "Enter a pattern.";
			else if (!IsRegexPatternValid(Text))
				textError = "Enter a valid pattern.";
		}

		// 2. 
		TextError = textError;
		HasErrors = TextError != null;
	}

	private static bool IsRegexPatternValid(string pattern)
	{
		try
		{
			_ = new System.Text.RegularExpressions.Regex(pattern);
			return true;
		}
		catch
		{
			return false;
		}
	}

	#endregion
}