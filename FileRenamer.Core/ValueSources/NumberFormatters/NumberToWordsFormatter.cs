using System.Xml;
using Humanizer;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.ValueSources.NumberFormatters;

public sealed class NumberToWordsFormatter : INumberFormatter
{
	/// <summary>neuter</summary>
	private const GrammaticalGender defaultGender = GrammaticalGender.Neuter;


	public string Description => "counter written at length";

	/// <summary>
	/// Gets or sets a value indicating whether to use uppercase letters.
	/// </summary>
	/// <remarks>Default value is <see langword="true"/>.</remarks>
	public bool UseUppercase { get; set; } = true;

	/// <summary>
	/// Gets or sets the grammatical gender of the generated text.
	/// <br/>
	/// <br/>
	/// The default gender is <inheritdoc cref="defaultGender"/>.
	/// </summary>
	public GrammaticalGender Gender { get; set; } = defaultGender;


	public string Format(int n)
	{
		string words = n.ToWords(Gender);

		return UseUppercase
			 ? words.ToUpperInvariant()
			 : words.ToLowerInvariant();
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(UseUppercase), UseUppercase).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(Gender), Gender).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<INumberFormatter> ReadXmlAsync(XmlReader reader)
	{
		bool? useUppercase = null;
		GrammaticalGender? gender = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(UseUppercase):
					useUppercase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(Gender):
					gender = Enum.Parse<GrammaticalGender>(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(NumberToWordsFormatter));

		//
		XmlSerializationHelper.ThrowIfNull(useUppercase, nameof(UseUppercase));
		XmlSerializationHelper.ThrowIfNull(gender, nameof(Gender));

		NumberToWordsFormatter result = new()
		{
			UseUppercase = useUppercase.Value,
			Gender = gender.Value,
		};

		return Task.FromResult(result as INumberFormatter);
	}

	#endregion
}