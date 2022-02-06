using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")] 
#endif
public sealed class ToCaseAction : RenameActionBase
{
	private readonly IIndex startIndexFinder;
	private readonly IIndex endIndexFinder;
	private readonly TextCasing textCase;


	public ToCaseAction(IIndex startIndexFinder, IIndex endIndexFinder, TextCasing textCase)
	{
		this.startIndexFinder = startIndexFinder;
		this.endIndexFinder = endIndexFinder;
		this.textCase = textCase;

		string range = (this.startIndexFinder, this.endIndexFinder) switch
		{
			(BeginningIndexFinder, EndIndexFinder) => $"all characters",
			(BeginningIndexFinder, FileExtensionIndexFinder) => $"file name",
			(FileExtensionIndexFinder, EndIndexFinder) => $"file extension",
			_ => $"characters from {this.startIndexFinder.Description.ToString(includePreposition: false)} to {this.endIndexFinder.Description.ToString(includePreposition: false)}",
		};

		string @case = this.textCase switch
		{
			TextCasing.LowerCase => "lowercase",
			TextCasing.UpperCase => "uppercase",
			TextCasing.SentenceCase => "sentence case",
			TextCasing.TitleCase => "title case",
			TextCasing.TitleCaseIgnoreCommonWords => "title case (ignore common words)",
			_ => this.textCase.ToString(),
		};

		Description = $"convert {range} to {@case}";
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int startIndex = startIndexFinder.FindIn(input);

		if (startIndex < 0 || input.Length <= startIndex)
			return input;

		// 1.3. 
		int endIndex = endIndexFinder.FindIn(input);

		if (endIndex < startIndex)
			return input;

		// 2. 
		string s1 = input[..startIndex];
		string s2 = input[startIndex..endIndex].ToCase(textCase);
		string s3 = input[endIndex..];

		// 3. 
		return s1 + s2 + s3;
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return new ToCaseAction(startIndexFinder, endIndexFinder, textCase);
	}
}