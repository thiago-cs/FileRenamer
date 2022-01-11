namespace FileRenamer.Core.Indices;

public sealed class IndexFinderDescription
{
	private readonly string? startPrepositon;
	private readonly string description;


	public IndexFinderDescription(string? startPrepositon, string description)
	{
		this.startPrepositon = startPrepositon;
		this.description = description;
	}

	internal object ToString(bool includePreposition)
	{
		return includePreposition && !string.IsNullOrWhiteSpace(startPrepositon)
			? $"{startPrepositon} {description}"
			: description;
	}
}