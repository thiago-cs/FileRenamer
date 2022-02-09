namespace FileRenamer.Core.Indices;

public sealed class IndexDescription
{
	private readonly string? startPrepositon;
	private readonly string description;


	public IndexDescription(string? startPrepositon, string description)
	{
		this.startPrepositon = startPrepositon;
		this.description = description;
	}

	public override string ToString()
	{
		return ToString(true);
	}

	public string ToString(bool includePreposition)
	{
		return includePreposition && !string.IsNullOrWhiteSpace(startPrepositon)
			? $"{startPrepositon} {description}"
			: description;
	}
}