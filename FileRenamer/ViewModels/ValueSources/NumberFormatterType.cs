using System.ComponentModel;


namespace FileRenamer.ViewModels.ValueSources;

public enum NumberFormatterType
{
	[Description("padded number")]
	PaddedNumberFormatter,

	[Description("roman number")]
	RomanNumberFormatter,

	[Description("words")]
	NumberToWordsFormatter,
}