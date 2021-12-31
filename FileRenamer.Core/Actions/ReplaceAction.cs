namespace FileRenamer.Core.Actions;

public sealed class ReplaceAction : RenameActionBase
{
	private readonly string oldString;
	private readonly string newString;
	private readonly bool ignoreCase;


	public ReplaceAction(string oldString, string newString, bool ignoreCase)
	{
		this.oldString = oldString;
		this.newString = newString;
		this.ignoreCase = ignoreCase;
	}


	public override string Run(string input)
	{
		return input.Replace(oldString, newString, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
	}
}