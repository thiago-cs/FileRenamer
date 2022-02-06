namespace FileRenamer.Core.Extensions;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Actions.ToCaseAction.ToCaseAction(Indices.IIndex, Indices.IIndex, TextCasing)"/>
/// <seealso cref="StringExtensions.ToCase(string, TextCasing)"/>
public enum TextCasing
{
	LowerCase,
	UpperCase,
	SentenceCase,
	TitleCase,
	TitleCaseIgnoreCommonWords,
}