using Humanizer;


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
}