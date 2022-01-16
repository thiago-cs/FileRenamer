namespace FileRenamer.Core.Extensions;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Actions.ToCaseAction.ToCaseAction(Indices.IIndexFinder, Indices.IIndexFinder, TextCasing)"/>
/// <seealso cref="StringExtensions.ToCase(string, TextCasing)"/>
public enum TextCasing
{
	LowerCase,
	UpperCase,
	SentenceCase,
	TitleCase,
	TitleCaseIgnoreCommonWords,
}