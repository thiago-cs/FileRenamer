using System.Xml;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.ValueSources;

public sealed class FolderNameValueSource : IValueSource
{
	public string Description => $"the folder name";


	public string GetValue(JobTarget target)
	{
		return Path.GetFileName(target.FolderName);
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);
		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IValueSource> ReadXmlAsync(XmlReader reader)
	{
		reader.ReadStartElement(nameof(FolderNameValueSource));

		return Task.FromResult(new FolderNameValueSource() as IValueSource);
	}

	#endregion
}