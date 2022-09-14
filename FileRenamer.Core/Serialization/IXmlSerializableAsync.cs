using System.Xml;


namespace FileRenamer.Core.Serialization;

public interface IXmlSerializableAsync//<T>
{
	public Task WriteXmlAsync(XmlWriter writer);
	//public static Task<T> ReadXmlAsync(XmlReader reader);
}