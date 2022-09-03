namespace FileRenamer.Core.ValueSources.NumberFormatters;

public sealed class PaddedNumberFormatter : INumberFormatter
{
	public const int DefaultWidth = 2;
	public const char DefaultPaddingChar = '0';


	public string Description => $"counter";

	/// <summary>
	/// Gets or sets the minimum number of characters in the resulting string.
	/// </summary>
	public int MinWidth { get; set; } = DefaultWidth;

	/// <summary>
	/// Gets or sets the A Unicode padding character.
	/// </summary>
	public char PaddingChar { get; set; } = DefaultPaddingChar;


	public string Format(int n)
	{
		return n.ToString().PadLeft(MinWidth, PaddingChar);
	}
}