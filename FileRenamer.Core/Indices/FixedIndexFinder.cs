namespace FileRenamer.Core.Indices;

public sealed class FixedIndexFinder : IIndexFinder
{
	private readonly int index;


	public FixedIndexFinder(int value)
	{
		index = value;
	}

	public static implicit operator FixedIndexFinder(int value) => new(value);


	public int FindIn(string input)
	{
		return index;
	}
}