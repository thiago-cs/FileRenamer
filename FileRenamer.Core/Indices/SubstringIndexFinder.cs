namespace FileRenamer.Core.Indices;

public sealed class SubstringIndexFinder : IIndexFinder
{
	private readonly string value;
	private readonly bool before;


	public SubstringIndexFinder(string value, bool before)
	{
		this.value = value;
		this.before = before;
	}


	public int FindIn(string input)
	{
		int index = input.IndexOf(value);

		return before || index == -1 ? index : index + value.Length;
	}
}