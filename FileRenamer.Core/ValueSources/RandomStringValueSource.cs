using FileRenamer.Core.Jobs;


namespace FileRenamer.Core.ValueSources;

public sealed class RandomStringValueSource : IValueSource
{
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
}