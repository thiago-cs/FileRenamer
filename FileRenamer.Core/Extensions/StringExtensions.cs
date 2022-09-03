using System.Text.RegularExpressions;


namespace FileRenamer.Core.Extensions;

public static class StringExtensions
{
	private static Regex? sentenceCaseRegex;
	private static Regex? titleCaseRegex;

	private static readonly HashSet<string> commomWords = new()
	{
		"a", "abaft", "about", "above", "afore", "after", "along", "amid", "among", "an", "and", "apud", "as", "aside", "at", "atop",
		"below", "but", "by",
		"circa",
		"down",
		"for", "from",
		"given",
		"in", "into",
		"lest", "like",
		"mid", "midst", "minus",
		"near", "next", "nor",
		"of", "off", "on", "onto", "or", "out", "over",
		"pace", "past", "per", "plus", "pro",
		"qua",
		"round",
		"sans", "save", "since", "so",
		"than", "the", "thru", "till", "times", "to",
		"under", "until", "unto", "up", "upon",
		"via", "vice",
		"with", "worth",
		"yet",
	};
	//{
	//	"a", "an", "and", "at",
	//	"but", "by",
	//	"for",
	//	"in",
	//	"nor",
	//	"of", "on", "or",
	//	"so",
	//	"the", "to",
	//	"up",
	//	"yet",
	//};


	public static string ToSentenceInvariant(this string s)
	{
		if (string.IsNullOrEmpty(s))
			return s;

		sentenceCaseRegex ??= new Regex(@"^[a-z]|\.\s+[a-z]", RegexOptions.ExplicitCapture);

		if (s == s.ToUpperInvariant())
			s = s.ToLowerInvariant();

		return sentenceCaseRegex.Replace(s, match => match.Value.ToUpperInvariant());
	}

	public static string ToTitleInvariant(this string s, bool ignoreCommonWords)
	{
		//
		if (string.IsNullOrEmpty(s))
			return s;

		//
		//titleCaseRegex ??= new Regex(@"(^[a-z])|\s([a-z])", RegexOptions.ExplicitCapture);
		titleCaseRegex = new Regex(@"\b[a-z]\w*", RegexOptions.ExplicitCapture);

		if (s == s.ToUpperInvariant())
			s = s.ToLowerInvariant();

		//
		MatchCollection matches =  titleCaseRegex.Matches(s);

		if (matches.Count == 0)
			return s;

		char[] array = s.ToCharArray();
		array[0] = char.ToUpperInvariant(array[0]);

		foreach (Match match in matches)
		{
			if (ignoreCommonWords && commomWords.Contains(match.Value))
				continue;

			array[match.Index] = char.ToUpperInvariant(array[match.Index]);
		}

		return new(array);
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
			TextCasing.TitleCase => s.ToTitleInvariant(ignoreCommonWords: false),
			TextCasing.TitleCaseIgnoreCommonWords => s.ToTitleInvariant(ignoreCommonWords: true),
			_ => throw new NotImplementedException($"Case for {textCase} is not implemented."),
		};
	}

	public static string Reverse(this string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}
}