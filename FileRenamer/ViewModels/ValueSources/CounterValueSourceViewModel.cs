using FileRenamer.Core.ValueSources;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.ViewModels.ValueSources;

internal sealed class CounterValueSourceViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableValidator, IValueSourceViewModel
{
	public static readonly NumberFormatterType[] ValueSourceTypes = System.Enum.GetValues<NumberFormatterType>();


	public int InitialValue { get; set; }

	public int Increment { get; set; }

	private NumberFormatterType? _numberFormatterType;
	public NumberFormatterType? NumberFormatterType
	{
		get => _numberFormatterType;

		set
		{
			if (SetProperty(ref _numberFormatterType, value))
				Formatter = ToNumberFormatter(value);
		}
	}

	private INumberFormatter _formatter;
	public INumberFormatter Formatter { get => _formatter; set => SetProperty(ref _formatter, value); }

	public IValueSource ValueSource => new CounterValueSource() { InitialValue = InitialValue, Increment = Increment, Formatter = Formatter };


	public CounterValueSourceViewModel() : this(new())
	{ }

	public CounterValueSourceViewModel(CounterValueSource valueSource)
	{
		InitialValue = valueSource.InitialValue;
		Increment = valueSource.Increment;
		NumberFormatterType = ToNumberFormatterType(valueSource.Formatter);
		Formatter = valueSource.Formatter;
	}


	private static INumberFormatter ToNumberFormatter(NumberFormatterType? value)
	{
		return value switch
		{
			ValueSources.NumberFormatterType.PaddedNumberFormatter => new PaddedNumberFormatter(),
			ValueSources.NumberFormatterType.RomanNumberFormatter => new RomanNumberFormatter(),
			ValueSources.NumberFormatterType.NumberToWordsFormatter => new NumberToWordsFormatter(),
			null => null,
			_ => throw new System.NotImplementedException(),
		};
	}

	private static NumberFormatterType? ToNumberFormatterType(INumberFormatter formatter)
	{
		return formatter switch
		{
			PaddedNumberFormatter => ValueSources.NumberFormatterType.PaddedNumberFormatter,
			RomanNumberFormatter => ValueSources.NumberFormatterType.RomanNumberFormatter,
			NumberToWordsFormatter => ValueSources.NumberFormatterType.NumberToWordsFormatter,
			null => null,
			_ => throw new System.NotImplementedException(),
		};
	}
}