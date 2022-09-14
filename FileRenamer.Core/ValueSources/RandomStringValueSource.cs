using System.Xml;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.ValueSources;

public sealed class RandomStringValueSource : IValueSource
{
	#region Properties and fields

	public const int DefaultLength = 8;
	public const int MaxLength = 100;
	/// <summary>'!', '@', '#', '$', '%', '&amp;'</summary>
	public const string DefaultSymbols = "!@#$%&";

	private Random? random;


	public string Description
	{
		get
		{
			System.Text.StringBuilder builder = new();

			builder.Append("a ")
				   .Append(Length)
				   .Append("-char string using [");

			if (IncludeLowercase)
				builder.Append("a-z");

			if (IncludeUppercase)
				builder.Append("A-Z");

			if (IncludeUppercase)
				builder.Append("0-9");

			if (IncludeSymbols)
				builder.Append(Symbols);

			builder.Append(']');


			return builder.ToString();
		}
	}

	private int _length = DefaultLength;
	/// <summary>
	/// Gets or sets the length of the resulting string.
	/// </summary>
	/// <remarks>The maximum length for the generated string is defined by <see cref="MaxLength"/>.</remarks>
	public int Length { get => _length; set => _length = Math.Min(value, MaxLength); }

	/// <summary>
	/// Gets or sets a value that indicates whether to include lowercase characters in the resulting string.
	/// </summary>
	/// <remarks>Default value is <see langword="true"/>.</remarks>
	public bool IncludeLowercase { get; set; } = true;

	/// <summary>
	/// Gets or sets a value that indicates whether to include uppercase characters in the resulting string.
	/// </summary>
	/// <remarks>Default value is <see langword="true"/>.</remarks>
	public bool IncludeUppercase { get; set; } = true;

	/// <summary>
	/// Gets or sets a value that indicates whether to include numbers (0-9) in the resulting string.
	/// </summary>
	/// <remarks>Default value is <see langword="true"/>.</remarks>
	public bool IncludeNumbers { get; set; } = true;

	/// <summary>
	/// Gets or sets a value that indicates whether to include symbols in the resulting string.
	/// </summary>
	/// <remarks>Default value is <see langword="true"/>.</remarks>
	public bool IncludeSymbols { get; set; } = false;

	/// <summary>
	/// Gets or sets the extra characters to include in the resulting string.
	/// <br/>
	/// <br/>
	/// Default value includes <inheritdoc cref="DefaultSymbols"/>.
	/// </summary>
	public string Symbols { get; set; } = DefaultSymbols;

	#endregion


	public string GetValue(JobTarget target)
	{
		return GetValue();
	}

	public string GetValue()
	{
		// 0.
		if (Length <= 0)
			return string.Empty;

		//
		random ??= new Random();

		//
		List<char> charPool = new(100);

		if (IncludeLowercase)
			for (char c = 'a'; c <= 'z'; c++)
				charPool.Add(c);

		if (IncludeUppercase)
			for (char c = 'A'; c <= 'Z'; c++)
				charPool.Add(c);

		if (IncludeUppercase)
			for (char c = '0'; c <= '9'; c++)
				charPool.Add(c);

		if (IncludeSymbols)
			foreach (char c in Symbols)
				charPool.Add(c);

		//
		char[] chars = new char[Length];

		for (int i = 0; i < Length; i++)
			chars[i] = charPool[random.Next(charPool.Count)];

		//
		return new(chars);
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(Length), Length).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IncludeLowercase), IncludeLowercase).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IncludeUppercase), IncludeUppercase).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IncludeNumbers), IncludeNumbers).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IncludeSymbols), IncludeSymbols).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(Symbols), Symbols).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IValueSource> ReadXmlAsync(XmlReader reader)
	{
		int? length = null;
		bool? includeLowercase = null;
		bool? includeUppercase = null;
		bool? includeNumbers = null;
		bool? includeSymbols = null;
		string? symbols = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(Length):
					length = int.Parse(reader.Value);
					break;

				case nameof(IncludeLowercase):
					includeLowercase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(IncludeUppercase):
					includeUppercase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(IncludeNumbers):
					includeNumbers = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(IncludeSymbols):
					includeSymbols = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(Symbols):
					symbols = reader.Value;
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(RandomStringValueSource));

		//
		XmlSerializationHelper.ThrowIfNull(length, nameof(Length));
		XmlSerializationHelper.ThrowIfNull(includeLowercase, nameof(IncludeLowercase));
		XmlSerializationHelper.ThrowIfNull(includeUppercase, nameof(IncludeUppercase));
		XmlSerializationHelper.ThrowIfNull(includeNumbers, nameof(IncludeNumbers));
		XmlSerializationHelper.ThrowIfNull(includeSymbols, nameof(IncludeSymbols));
		XmlSerializationHelper.ThrowIfNull(symbols, nameof(Symbols));

		RandomStringValueSource result = new()
		{
			Length = length.Value,
			IncludeLowercase = includeLowercase.Value,
			IncludeUppercase = includeUppercase.Value,
			IncludeNumbers = includeNumbers.Value,
			IncludeSymbols = includeSymbols.Value,
			Symbols = symbols,
		};

		return Task.FromResult(result as IValueSource);
	}

	#endregion
}