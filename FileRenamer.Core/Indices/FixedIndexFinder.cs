namespace FileRenamer.Core.Indices;

public sealed class FixedIndexFinder : IIndexFinder
{
	private readonly int index;


	public IndexFinderDescription Description { get; private set; }


	public FixedIndexFinder(int value)
	{
		index = value;
		Description = new("", $"after char. #{value}");
	}

	public static implicit operator FixedIndexFinder(int value) => new(value);


	public int FindIn(string input)
	{
		return index;
	}
}