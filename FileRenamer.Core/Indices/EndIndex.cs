namespace FileRenamer.Core.Indices;

public sealed class EndIndex : IIndex
{
	public IndexDescription Description => new("at the", "end");


	public EndIndex() { }


	public int FindIn(string input)
	{
		return input.Length;
	}
}