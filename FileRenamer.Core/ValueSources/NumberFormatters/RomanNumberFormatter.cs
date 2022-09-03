namespace FileRenamer.Core.ValueSources.NumberFormatters;

public sealed class RomanNumberFormatter : INumberFormatter
{
	public string Description => "counter using Roman numerals";

	/// <summary>
	/// Gets or sets a value indicating whether to use uppercase letters.
	/// </summary>
	/// <remarks>Default value is <see langword="true"/>.</remarks>
	public bool UseUppercase { get; set; } = true;

	public string Format(int n)
	{
		string roman = Humanizer.RomanNumeralExtensions.ToRoman(n);

		return UseUppercase
			 ? roman.ToUpperInvariant()
			 : roman.ToLowerInvariant();
	}
}