using System.Text.RegularExpressions;


namespace FileRenamer.Core.Actions;

public sealed class ReplaceAction : RenameActionBase
{
	private readonly string oldString;
	private readonly string? newString;
	private readonly bool ignoreCase;
	private readonly bool useRegex;
	private Regex? regex;


	public ReplaceAction(string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		this.oldString = oldString;
		this.newString = newString;
		this.ignoreCase = ignoreCase;
		this.useRegex = useRegex;
	}


	public override string Run(string input)
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