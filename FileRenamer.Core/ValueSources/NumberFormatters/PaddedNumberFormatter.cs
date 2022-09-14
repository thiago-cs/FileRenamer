using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.ValueSources.NumberFormatters;

public sealed class PaddedNumberFormatter : INumberFormatter
{
	#region Properties and fields

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

	#endregion


	public string Format(int n)
	{
		return n.ToString().PadLeft(MinWidth, PaddingChar);
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(MinWidth), MinWidth).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(PaddingChar), PaddingChar).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<INumberFormatter> ReadXmlAsync(XmlReader reader)
	{
		int? minWidth = null;
		char? paddingChar = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(MinWidth):
					minWidth = int.Parse(reader.Value);
					break;

				case nameof(PaddingChar):
					paddingChar = reader.Value[0];
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(PaddedNumberFormatter));

		//
		XmlSerializationHelper.ThrowIfNull(minWidth, nameof(MinWidth));
		XmlSerializationHelper.ThrowIfNull(paddingChar, nameof(PaddingChar));

		PaddedNumberFormatter result = new()
		{
			MinWidth = minWidth.Value,
			PaddingChar = paddingChar.Value,
		};

		return Task.FromResult(result as INumberFormatter);
	}

	#endregion
}