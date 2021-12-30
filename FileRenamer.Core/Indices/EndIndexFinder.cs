namespace FileRenamer.Core.Indices;

public sealed class EndIndexFinder : IIndexFinder
{
	public int FindIn(string input)
	{
		return input.Length;
	}
}