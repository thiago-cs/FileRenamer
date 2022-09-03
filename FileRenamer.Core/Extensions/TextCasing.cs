using System.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.Core.Extensions;

/// <summary>
/// 
/// </summary>
/// Do not forget to update the following methods when adding/removing a value.
/// <seealso cref="ToCaseAction.ToCaseAction(Indices.IIndex, Indices.IIndex, TextCasing)"/>
/// <seealso cref="StringExtensions.ToCase(string, TextCasing)"/>
public enum TextCasing
{
	[Description("lowercase")]
	LowerCase,

	[Description("uppercase")]
	UpperCase,

	[Description("sentence case")]
	SentenceCase,

	[Description("title case")]
	TitleCase,

	[Description("title case (ignore common words)")]
	TitleCaseIgnoreCommonWords,
}