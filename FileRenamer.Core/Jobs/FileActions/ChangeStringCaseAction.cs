using System.Text.RegularExpressions;
using FileRenamer.Core.Extensions;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ChangeStringCaseAction : RenameActionBase
{
	private Regex? regex;

	public string OldString { get; set; }
	public bool IgnoreCase { get; set; }
	public bool UseRegex { get; set; }
	public TextCasing TextCase { get; }


	public ChangeStringCaseAction(string oldString, bool ignoreCase, bool useRegex, TextCasing textCase)
	{
		OldString = oldString;
		IgnoreCase = ignoreCase;
		UseRegex = useRegex;
		TextCase = textCase;

		UpdateDescription();
	}


	public override void Run(JobTarget target, JobContext context)
	{
		string input = target.NewFileName;

		//1. Plain text
		if (!UseRegex)
		{
			target.NewFileName = input.Replace(
				OldString,
				OldString.ToCase(TextCase),
				IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

			return;
		}

		// 2. REGEX
		// 2.1. Creates the Regex object, if necessary.
		if (regex == null)
		{
			RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

			if (IgnoreCase)
				regexOptions |= RegexOptions.IgnoreCase;

			regex = new(OldString, regexOptions);
		}

		// 2.2. Applies the case conversion.
		IList<Match> matches = regex.Matches(input);

		foreach (Match match in matches)
			if (match.Value.Length != 0)
				input = input.Replace(match.Value, match.Value.ToCase(TextCase));

		// 2.3. 
		target.NewFileName = input;
	}

	public override void UpdateDescription()
	{
		string @case = TextCase switch
		{
			TextCasing.LowerCase => "lowercase",
			TextCasing.UpperCase => "uppercase",
			TextCasing.SentenceCase => "sentence case",
			TextCasing.TitleCase => "title case",
			TextCasing.TitleCaseIgnoreCommonWords => "title case (ignore common words)",
			_ => TextCase.ToString(),
		};

		System.Text.StringBuilder sb = new();

		sb.Append("change all occurrences of ");

		if (UseRegex)
			sb.Append("the expression ");

		sb.Append('"')
		  .Append(OldString)
		  .Append(@""" to ")
		  .Append(@case);

		if (IgnoreCase)
			sb.Append(@" (ignore case)");

		Description = sb.ToString();
	}

	public override RenameActionBase DeepCopy()
	{
		return new ChangeStringCaseAction(OldString, IgnoreCase, UseRegex, TextCase);
	}
}