namespace FileRenamer.UserControls.InputControls;

internal class IndexFinderTypeEntry
{
	public IndexType Type { get; }
	public string Preposition { get; }
	public string DescriptionWithPreposition { get; }
	public string DescriptionWithoutPreposition { get; }


	public IndexFinderTypeEntry(IndexType type, string preposition, string descriptionWithoutPreposition)
	{
		Type = type;
		Preposition = preposition;
		DescriptionWithoutPreposition = descriptionWithoutPreposition;
		DescriptionWithPreposition = string.IsNullOrWhiteSpace(preposition) ? descriptionWithoutPreposition : $"{preposition} {descriptionWithoutPreposition}";
	}
}