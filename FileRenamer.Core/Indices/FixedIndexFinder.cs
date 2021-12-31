namespace FileRenamer.Core.Indices;

public sealed class FixedIndexFinder : IIndexFinder
{
	private readonly int index;


	public FixedIndexFinder(int value)
	{
		index = value;
	}


	public int FindIn(string input)
	{
		return index;
	}
}