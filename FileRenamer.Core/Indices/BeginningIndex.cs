namespace FileRenamer.Core.Indices;

public sealed class BeginningIndex : IIndex
{
	public IndexDescription Description { get; private set; }


	public BeginningIndex()
	{
		Description = new("at the", "beginning");
	}


	public int FindIn(string input)
	{
		return 0;
	}
}