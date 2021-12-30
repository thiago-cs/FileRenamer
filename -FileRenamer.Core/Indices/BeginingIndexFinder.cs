namespace FileRenamer.Core.Indices;

public sealed class BeginingIndexFinder : IIndexFinder
{
	int IIndexFinder.FindIndex(string s)
	{
		return 0;
	}
}