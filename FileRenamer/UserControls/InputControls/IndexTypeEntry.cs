using System.Linq;

namespace FileRenamer.UserControls.InputControls;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public class IndexTypeEntry
{
	public static readonly IndexTypeEntry[] IndexTypes =
	{
		new(IndexType.Beginning, "at", "the beginning"),
		new(IndexType.End, "at", "the end"),
		new(IndexType.FileExtension, null, "before extension"),
		new(IndexType.Position, "at", "position"),
		new(IndexType.Before, null, "before"),
		new(IndexType.After, null, "after"),
	};

	internal static IndexTypeEntry FromIndexType(IndexType indexType)
	{
		return IndexTypes.First(IndexTypeEntry => IndexTypeEntry.Type == indexType);
	}

	public IndexType Type { get; }
	public string Preposition { get; }
	public string Description { get; }
	public string DescriptionWithPreposition { get; }


	/// <summary>
	/// Initializes a new instance of the <see cref="IndexTypeEntry"/> class.
	/// </summary>
	/// <param name="type">A value that represents the type of the index.</param>
	/// <param name="preposition">The preposition that may be used before the description.</param>
	/// <param name="description">The description without preposition.</param>
	private IndexTypeEntry(IndexType type, string preposition, string description)
	{
		Type = type;
		Preposition = preposition;
		Description = description;
		DescriptionWithPreposition = string.IsNullOrWhiteSpace(preposition) ? description : $"{preposition} {description}";
	}


#if DEBUG
	private string GetDebuggerDisplay()
	{
		return $"Type = {Type}";
	}
#endif

	//public static implicit operator IndexType (IndexTypeEntry entry)
	//{
	//	return entry.Type;
	//}
}