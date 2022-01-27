namespace FileRenamer.UserControls.InputControls;

internal class IndexFinderTypeEntry
{
	public IndexFinderType Type { get; }
	public string Preposition { get; }
	public string DescriptionWithPreposition { get; }
	public string DescriptionWithoutPreposition { get; }


	public IndexFinderTypeEntry(IndexFinderType type, string preposition, string descriptionWithoutPreposition)
	{
		Type = type;
		Preposition = preposition;
		DescriptionWithoutPreposition = descriptionWithoutPreposition;
		DescriptionWithPreposition = string.IsNullOrWhiteSpace(preposition) ? descriptionWithoutPreposition : $"{preposition} {descriptionWithoutPreposition}";
	}
}