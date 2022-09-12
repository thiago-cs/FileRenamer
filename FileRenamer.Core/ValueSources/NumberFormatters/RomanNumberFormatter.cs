using System.Xml;
using FileRenamer.Core.Serialization;


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


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(UseUppercase), UseUppercase).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<INumberFormatter> ReadXmlAsync(XmlReader reader)
	{
		bool? useUppercase = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(UseUppercase):
					useUppercase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(RomanNumberFormatter));

		//
		XmlSerializationHelper.ThrowIfNull(useUppercase, nameof(UseUppercase));

		RomanNumberFormatter result = new() { UseUppercase = useUppercase.Value };
		return Task.FromResult(result as INumberFormatter);
	}

	#endregion
}