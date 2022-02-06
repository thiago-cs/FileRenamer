namespace FileRenamer.Core.Indices;

public sealed class FixedIndex : IIndex
{
	private readonly int index;


	public IndexDescription Description { get; private set; }


	public FixedIndex(int value)
	{
		index = value;
		Description = new("after", $"char. #{value}");
	}

	public static implicit operator FixedIndex(int value) => new(value);


	public int FindIn(string input)
	{
		return index;
	}
}