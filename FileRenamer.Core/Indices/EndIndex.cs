namespace FileRenamer.Core.Indices;

public sealed class EndIndex : IIndex
{
	public IndexDescription Description { get; private set; }


	public EndIndex()
	{
		Description = new("at the", "end");
	}


	public int FindIn(string input)
	{
		return input.Length;
	}
}