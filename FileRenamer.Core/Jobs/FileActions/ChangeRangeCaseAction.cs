﻿using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ChangeRangeCaseAction : RenameActionBase
{
	public IIndex StartIndex { get; set; }
	public IIndex EndIndex { get; set; }
	public TextCasing TextCase { get; }


	public ChangeRangeCaseAction(IIndex startIndex, IIndex endIndex, TextCasing textCase)
	{
		StartIndex = startIndex;
		EndIndex = endIndex;
		TextCase = textCase;

		UpdateDescription();
	}


	public override void Run(JobTarget target, JobContext context)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return;

		// 1.2. 
		string input = target.NewFileName;

		int startIndex = StartIndex.FindIn(input);

		if (startIndex < 0 || input.Length <= startIndex)
			return;

		// 1.3. 
		int endIndex = EndIndex.FindIn(input);

		if (endIndex < startIndex)
			return;

		// 2. 
		string s1 = input[..startIndex];
		string s2 = input[startIndex..endIndex].ToCase(TextCase);
		string s3 = input[endIndex..];

		// 3. 
		target.NewFileName = s1 + s2 + s3;
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

		Description = $"change {Indices.Range.GetDescription(StartIndex, EndIndex)} to {@case}";
	}

	public override RenameActionBase DeepCopy()
	{
		return new ChangeRangeCaseAction(StartIndex, EndIndex, TextCase);
	}
}