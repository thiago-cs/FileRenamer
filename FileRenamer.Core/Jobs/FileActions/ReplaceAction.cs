using System.Text.RegularExpressions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ReplaceAction : RenameActionBase
{
	private Regex? regex;

	public IIndex? StartIndex { get; set; }
	public IIndex? EndIndex { get; set; }
	public string OldString { get; set; }
	public string? NewString { get; set; }
	public bool IgnoreCase { get; set; }
	public bool UseRegex { get; set; }


	public ReplaceAction(string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		this.OldString = oldString ?? throw new ArgumentNullException(nameof(oldString));
		this.NewString = newString;
		this.IgnoreCase = ignoreCase;
		this.UseRegex = useRegex;

		UpdateDescription();
	}

	public ReplaceAction(IIndex startIndex, IIndex endIndex, string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		// 1. 
		this.StartIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		this.EndIndex = endIndex ?? throw new ArgumentNullException(nameof(endIndex));
		this.OldString = oldString ?? throw new ArgumentNullException(nameof(oldString));
		this.NewString = newString;
		this.IgnoreCase = ignoreCase;
		this.UseRegex = useRegex;

		UpdateDescription();
	}


	public override void Run(JobTarget target, JobContext context)
	{
		string input = target.NewFileName;

		// 
		if (this.StartIndex == null || this.EndIndex == null)
		{
			target.NewFileName = Run_Core(input);
			return;
		}

		//
		int startIndex = this.StartIndex.FindIn(input);

		if (startIndex == -1)
			return;

		int endIndex = this.EndIndex.FindIn(input);

		if (endIndex == -1)
			return;

		if (endIndex < startIndex)
			return;

		// 
		target.NewFileName = input[..startIndex]
						   + Run_Core(input[startIndex..endIndex])
						   + input[endIndex..];
		return;
	}

	public override void UpdateDescription()
	{
		if (StartIndex == null || EndIndex == null)
		{
			System.Text.StringBuilder sb = new();
			bool empty = string.IsNullOrEmpty(NewString);

			sb.Append(empty ? "remove all occurrencies of " : "replace ");

			if (UseRegex)
				sb.Append("the expression ");

			sb.Append('"')
			  .Append(OldString)
			  .Append('"');

			if (!empty)
				sb.Append(@" with """).Append(NewString).Append('"');

			Description = sb.ToString();
		}
		else
		{
			System.Text.StringBuilder sb = new("replace ");

			if (UseRegex)
				sb.Append("the expression ");

			sb.Append('"')
			  .Append(OldString)
			  .Append(@""" within ")
			  .Append(Indices.Range.GetDescription(StartIndex, EndIndex))
			  .Append(@" with """);

			if (NewString != null)
				sb.Append(NewString);

			sb.Append('"');

			Description = sb.ToString();
		}
	}

	public override RenameActionBase DeepCopy()
	{
		return StartIndex != null && EndIndex != null
				? new ReplaceAction(StartIndex, EndIndex, OldString, NewString, IgnoreCase, UseRegex)
				: new ReplaceAction(OldString, NewString, IgnoreCase, UseRegex);
	}

	private string Run_Core(string input)
	{
		if (UseRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (IgnoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(OldString, regexOptions);
			}

			return regex.Replace(input, NewString ?? "");
		}
		else
			return input.Replace(OldString, NewString, IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
	}
}