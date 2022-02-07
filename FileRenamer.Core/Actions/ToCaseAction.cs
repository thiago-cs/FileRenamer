using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")] 
#endif
public sealed class ToCaseAction : RenameActionBase
{
	private readonly IIndex startIndex;
	private readonly IIndex endIndex;
	private readonly TextCasing textCase;


	public ToCaseAction(IIndex startIndex, IIndex endIndex, TextCasing textCase)
	{
		this.startIndex = startIndex;
		this.endIndex = endIndex;
		this.textCase = textCase;

		UpdateDescription();
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int startIndex = this.startIndex.FindIn(input);

		if (startIndex < 0 || input.Length <= startIndex)
			return input;

		// 1.3. 
		int endIndex = this.endIndex.FindIn(input);

		if (endIndex < startIndex)
			return input;

		// 2. 
		string s1 = input[..startIndex];
		string s2 = input[startIndex..endIndex].ToCase(textCase);
		string s3 = input[endIndex..];

		// 3. 
		return s1 + s2 + s3;
	}

	public override void UpdateDescription()
	{
		string range = (startIndex, endIndex) switch
		{
			(BeginningIndex, EndIndex) => $"all characters",
			(BeginningIndex, FileExtensionIndex) => $"file name",
			(FileExtensionIndex, EndIndex) => $"file extension",
			_ => $"characters from {startIndex.Description.ToString(includePreposition: false)} to {endIndex.Description.ToString(includePreposition: false)}",
		};

		string @case = textCase switch
		{
			TextCasing.LowerCase => "lowercase",
			TextCasing.UpperCase => "uppercase",
			TextCasing.SentenceCase => "sentence case",
			TextCasing.TitleCase => "title case",
			TextCasing.TitleCaseIgnoreCommonWords => "title case (ignore common words)",
			_ => textCase.ToString(),
		};

		Description = $"convert {range} to {@case}";
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return new ToCaseAction(startIndex, endIndex, textCase);
	}
}