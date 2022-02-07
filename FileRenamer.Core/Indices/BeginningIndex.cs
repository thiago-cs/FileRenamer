namespace FileRenamer.Core.Indices;

public sealed class BeginningIndex : IIndex
{
	public IndexDescription Description => new("at the", "beginning");


	public BeginningIndex() { }


	public int FindIn(string input)
	{
		return 0;
	}
}