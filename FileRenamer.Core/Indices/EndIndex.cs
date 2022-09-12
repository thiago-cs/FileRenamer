using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Indices;

public sealed class EndIndex : IIndex
{
	public IndexDescription Description => new("at the", "end");


	public EndIndex() { }


	public int FindIn(string input)
	{
		return input.Length;
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);
		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IIndex> ReadXmlAsync(XmlReader reader)
	{
		reader.ReadStartElement(nameof(EndIndex));

		return Task.FromResult(new EndIndex() as IIndex);
	}

	#endregion
}