using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.ValueSources;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.ViewModels.ValueSources;

internal sealed partial class CounterValueSourceViewModel : ObservableValidator, IValueSourceViewModel
{
	public static readonly NumberFormatterType[] ValueSourceTypes = System.Enum.GetValues<NumberFormatterType>();


	public int InitialValue { get; set; }

	public int Increment { get; set; }

	[ObservableProperty]
	private NumberFormatterType? _numberFormatterType;

	partial void OnNumberFormatterTypeChanged(NumberFormatterType? value)
	{
		Formatter = ToNumberFormatter(value);
	}

	[ObservableProperty]
	private INumberFormatter _formatter;

	public IValueSource ValueSource => new CounterValueSource() { InitialValue = InitialValue, Increment = Increment, Formatter = Formatter };


	#region Constructors

	public CounterValueSourceViewModel() : this(new())
	{ }

	public CounterValueSourceViewModel(CounterValueSource valueSource)
	{
		InitialValue = valueSource.InitialValue;
		Increment = valueSource.Increment;
		NumberFormatterType = ToNumberFormatterType(valueSource.Formatter);
		Formatter = valueSource.Formatter;
	}

	#endregion


	#region Private helpers

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

	#endregion
}