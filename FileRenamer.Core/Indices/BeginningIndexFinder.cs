namespace FileRenamer.Core.Indices;

public sealed class BeginningIndexFinder : IIndexFinder
{
	public IndexFinderDescription Description { get; private set; }


	public BeginningIndexFinder()
	{
		Description = new("at the", "beginning");
	}


	public int FindIn(string input)
	{
		return 0;
	}
}