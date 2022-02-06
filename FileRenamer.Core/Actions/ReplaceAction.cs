using System.Text.RegularExpressions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ReplaceAction : RenameActionBase
{
	private readonly IIndex? startIndex;
	private readonly IIndex? endIndex;
	private readonly string oldString;
	private readonly string? newString;
	private readonly bool ignoreCase;
	private readonly bool useRegex;
	private Regex? regex;


	public ReplaceAction(string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		// 1. 
		this.oldString = oldString ?? throw new ArgumentNullException(nameof(oldString));
		this.newString = newString;
		this.ignoreCase = ignoreCase;
		this.useRegex = useRegex;

		// 2.
		System.Text.StringBuilder sb = new();
		bool empty = string.IsNullOrEmpty(this.newString);

		sb.Append(empty ? "remove all occurrencies of " : "replace ");

		if (this.useRegex)
			sb.Append("the expression ");

		sb.Append('"')
		  .Append(this.oldString)
		  .Append('"');

		if (!empty)
			sb.Append(@" with """).Append(this.newString).Append('"');

		Description = sb.ToString();
	}

	public ReplaceAction(IIndex startIndex, IIndex endIndex, string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		// 1. 
		this.startIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		this.endIndex = endIndex ?? throw new ArgumentNullException(nameof(endIndex));
		this.oldString = oldString ?? throw new ArgumentNullException(nameof(oldString));
		this.newString = newString;
		this.ignoreCase = ignoreCase;
		this.useRegex = useRegex;

		// 2. 
		System.Text.StringBuilder sb = new("replace ");

		if (this.useRegex)
			sb.Append("the expression ");

		sb.Append('"')
		  .Append(this.oldString)
		  .Append(@""" within ")
		  .Append(Helpers.DescriptionHelper.GetRangeFriendlyName(this.startIndex, this.endIndex))
		  .Append(@" with """);

		if (this.newString != null)
			sb.Append(this.newString);

		sb.Append('"');

		Description = sb.ToString();
	}


	public override string Run(string input)
	{
		// 
		if (this.startIndex == null || this.endIndex == null)
			return Run_Core(input);

		//
		int startIndex = this.startIndex.FindIn(input);

		if (startIndex == -1)
			return input;

		int endIndex = this.endIndex.FindIn(input);

		if (endIndex == -1)
			return input;

		if (endIndex < startIndex)
			return input;

		// 
		return input[..startIndex] + Run_Core(input[startIndex..endIndex]) + input[endIndex..];
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return startIndex != null && endIndex != null
				? new ReplaceAction(startIndex, endIndex, oldString, newString, ignoreCase, useRegex)
				: new ReplaceAction(oldString, newString, ignoreCase, useRegex);
	}

	private string Run_Core(string input)
	{
		if (useRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (ignoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(oldString, regexOptions);
			}

			return regex.Replace(input, newString ?? "");
		}
		else
			return input.Replace(oldString, newString, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
	}
}