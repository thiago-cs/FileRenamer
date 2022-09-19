using FileRenamer.Core.ValueSources;


namespace FileRenamer.ViewModels.ValueSources;

internal sealed class RandomStringValueSourceViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableValidator, IValueSourceViewModel
{
	public int Length { get; set; }

	public bool IncludeLowercase { get; set; }

	public bool IncludeUppercase { get; set; }

	public bool IncludeNumbers { get; set; }

	public bool IncludeSymbols { get; set; }

	public string Symbols { get; set; }

	public IValueSource ValueSource => new RandomStringValueSource() { Length = Length, IncludeLowercase = IncludeLowercase, IncludeUppercase = IncludeUppercase, IncludeNumbers = IncludeNumbers, IncludeSymbols = IncludeSymbols, Symbols = Symbols, };


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
}