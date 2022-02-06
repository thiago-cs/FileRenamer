namespace FileRenamer.Core.Indices;

public sealed class EndIndexFinder : IIndex
{
	public IndexFinderDescription Description { get; private set; }


	public EndIndexFinder()
	{
		Description = new("at the", "end");
	}


	public int FindIn(string input)
	{
		return input.Length;
	}
}