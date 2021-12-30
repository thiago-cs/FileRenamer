namespace FileRenamer.Core.Indices;

public sealed class EndIndexFinder : IIndexFinder
{
	int IIndexFinder.FindIndex(string s)
	{
		return s.Length;
	}
}