namespace FileRenamer.Core.Indices;

public sealed class SubstringIndexFinder : IIndexFinder
{
	private readonly string value;
	private readonly bool before;
	private readonly bool ignoreCase;


	public SubstringIndexFinder(string value, bool before, bool ignoreCase)
	{
		this.value = value;
		this.before = before;
		this.ignoreCase = ignoreCase;
	}


	public int FindIn(string input)
	{
		int index = input.IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

		return before || index == -1 ? index : index + value.Length;
	}
}