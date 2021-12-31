using System.Text.RegularExpressions;


namespace FileRenamer.Core.Extensions;

public static class StringExtensions
{
	private static Regex? sentenceCaseRegex;
	private static Regex? titleCaseRegex;
	
	
	public static string ToSentenceInvariant(this string s)
	{
		if (string.IsNullOrEmpty(s))
			return s;

		sentenceCaseRegex ??= new Regex(@"(^[a-z])|\.\s+([a-z])", RegexOptions.ExplicitCapture);

		if (s == s.ToUpperInvariant())
			s = s.ToLowerInvariant();

		return sentenceCaseRegex.Replace(s, match => match.Value.ToUpperInvariant());
	}

	public static string ToTitleInvariant(this string s)
	{
		if (string.IsNullOrEmpty(s))
			return s;

		titleCaseRegex ??= new Regex(@"(^[a-z])|\s([a-z])", RegexOptions.ExplicitCapture);

		if (s == s.ToUpperInvariant())
			s = s.ToLowerInvariant();

		return titleCaseRegex.Replace(s, match => match.Value.ToUpperInvariant());
	}

	public static string ToCase(this string s, TextCasing textCase)
	{
		if (string.IsNullOrEmpty(s))
			return s;

		return textCase switch
		{
			TextCasing.LowerCase => s.ToLowerInvariant(),
			TextCasing.UpperCase => s.ToUpperInvariant(),
			TextCasing.SentenceCase => s.ToSentenceInvariant(),
			TextCasing.TitleCase => s.ToTitleInvariant(),
			_ => throw new NotImplementedException($"Case for {textCase} is not implemented."),
		};
	}
}