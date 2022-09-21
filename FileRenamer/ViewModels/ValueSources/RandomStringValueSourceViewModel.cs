using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.ViewModels.ValueSources;

public sealed partial class RandomStringValueSourceViewModel : ObservableValidator, IValueSourceViewModel
{
	#region Properties

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(RandomStringValueSourceViewModel), nameof(ValidateLength))]
	private int _length;

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(RandomStringValueSourceViewModel), nameof(ValidateOptions))]
	private bool _includeLowercase;

	[ObservableProperty]
	private bool _includeUppercase;

	partial void OnIncludeUppercaseChanged(bool value)
	{
		ValidateIncludeLowercase();
	}

	[ObservableProperty]
	private bool _includeNumbers;

	partial void OnIncludeNumbersChanged(bool value)
	{
		ValidateIncludeLowercase();
	}

	[ObservableProperty]
	private bool _includeSymbols;

	partial void OnIncludeSymbolsChanged(bool value)
	{
		ValidateIncludeLowercase();
		ValidateProperty(Symbols, nameof(Symbols));
	}

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(RandomStringValueSourceViewModel), nameof(ValidateSymbols))]
	private string _symbols;

	private string lastUniqueSymbols;

	partial void OnSymbolsChanged(string value)
	{
		if (value != lastUniqueSymbols)
			Symbols = lastUniqueSymbols = new(value.ToCharArray().Distinct().ToArray());
	}

	public IValueSource ValueSource => new RandomStringValueSource()
	{
		Length = Length,
		IncludeLowercase = IncludeLowercase,
		IncludeUppercase = IncludeUppercase,
		IncludeNumbers = IncludeNumbers,
		IncludeSymbols = IncludeSymbols,
		Symbols = Symbols,
	};

	#endregion


	#region Constructors

	public RandomStringValueSourceViewModel() : this(new())
	{ }

	public RandomStringValueSourceViewModel(RandomStringValueSource valueSource)
	{
		Length = valueSource.Length;
		IncludeLowercase = valueSource.IncludeLowercase;
		IncludeUppercase = valueSource.IncludeUppercase;
		IncludeNumbers = valueSource.IncludeNumbers;
		IncludeSymbols = valueSource.IncludeSymbols;
		Symbols = valueSource.Symbols;
	}

	#endregion


	#region Validation

	private const string StringTooShortErrorMessage = "Select a number greater than 0.";
	private const string NoSelectedOptionsErrorMessage = "Select at least 1 option.";
	private const string NotEnoughSymbolsErrorMessage = "Enter at least 2 different symbols.";


	[ObservableProperty]
	private string _optionsErrorMessage;

	[ObservableProperty]
	private string _symbolsErrorMessage;


	private void ValidateIncludeLowercase()
	{
		ValidateProperty(IncludeLowercase, nameof(IncludeLowercase));
	}

	public static ValidationResult ValidateLength(int value, ValidationContext context)
	{
		RandomStringValueSourceViewModel instance = context.ObjectInstance as RandomStringValueSourceViewModel;

		return 1 <= instance.Length
			 ? ValidationResult.Success
			 : new ValidationResult(StringTooShortErrorMessage);
	}

	public static ValidationResult ValidateOptions(bool value, ValidationContext context)
	{
		RandomStringValueSourceViewModel instance = context.ObjectInstance as RandomStringValueSourceViewModel;

		string errorMessage = instance.IncludeLowercase || instance.IncludeUppercase || instance.IncludeNumbers || instance.IncludeSymbols
			 ? null
			 : NoSelectedOptionsErrorMessage;

		instance.OptionsErrorMessage = errorMessage;

		return errorMessage == null
			 ? ValidationResult.Success
			 : new ValidationResult(errorMessage);
	}

	public static ValidationResult ValidateSymbols(string value, ValidationContext context)
	{
		RandomStringValueSourceViewModel instance = context.ObjectInstance as RandomStringValueSourceViewModel;

		string errorMessage = instance.IncludeSymbols && instance.Symbols.Length < 2
			 ? NotEnoughSymbolsErrorMessage
			 : null;

		instance.SymbolsErrorMessage = errorMessage;

		return errorMessage == null
			 ? ValidationResult.Success
			 : new ValidationResult(errorMessage);
	}

	#endregion
}