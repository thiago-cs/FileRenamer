using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Indices;

public sealed class BeginningIndex : IIndex
{
	public IndexDescription Description => new("at the", "beginning");


	public BeginningIndex() { }


	public int FindIn(string input)
	{
		return 0;
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);
		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IIndex> ReadXmlAsync(XmlReader reader)
	{
		reader.ReadStartElement(nameof(BeginningIndex));

		return Task.FromResult(new BeginningIndex() as IIndex);
	}

	#endregion
}