using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Indices;

public sealed class FileExtensionIndex : IIndex
{
	public IndexDescription Description => new("before", "file's extension");


	public FileExtensionIndex() { }


	public int FindIn(string input)
	{
		for (int i = input.Length - 1; i >= 0; i--)
			switch (input[i])
			{
				case '.': return i;
				case ' ': return -1;
			}

		return -1;
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);
		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IIndex> ReadXmlAsync(XmlReader reader)
	{
		reader.ReadStartElement(nameof(FileExtensionIndex));

		return Task.FromResult(new FileExtensionIndex() as IIndex);
	}

	#endregion
}