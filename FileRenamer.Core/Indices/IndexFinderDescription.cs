namespace FileRenamer.Core.Indices;

public sealed class IndexFinderDescription
{
	public string StartPrepositon { get; }
	public string Description { get; }


	public IndexFinderDescription(string startPrepositon, string description)
	{
		StartPrepositon = startPrepositon;
		Description = description;
	}

	internal object ToString(bool includePreposition)
	{
		return includePreposition && !string.IsNullOrWhiteSpace(StartPrepositon)
			? $"{StartPrepositon} {Description}"
			: Description;
	}
}