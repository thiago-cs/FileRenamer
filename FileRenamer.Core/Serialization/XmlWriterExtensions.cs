using System.Xml;


namespace FileRenamer.Core.Serialization;

public static class XmlWriterExtensions
{
	public static Task WriteAttributeAsync<T>(this XmlWriter writer, string name, T value)
	{
		return writer.WriteAttributeStringAsync(null, name, null, value?.ToString() ?? "<null>");
	}

	public static Task WriteStartElementAsync(this XmlWriter writer, string name)
	{
		return writer.WriteStartElementAsync(null, name, null);
	}

	public static async Task WriteElementAsync(this XmlWriter writer, string name, IXmlSerializableAsync value)
	{
		await writer.WriteStartElementAsync(name).ConfigureAwait(false);
		await value.WriteXmlAsync(writer);
		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}
}