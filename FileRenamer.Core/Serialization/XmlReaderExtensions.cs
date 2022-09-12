using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.ValueSources.NumberFormatters;
using FileRenamer.Core.ValueSources;
using System.Xml;


namespace FileRenamer.Core.Serialization;

public static class XmlReaderExtensions
{
	public static async Task<IJobItem> ReadJobItemAsync(this XmlReader reader)
	{
		return reader.Name switch
		{
			nameof(InsertAction) => await InsertAction.ReadXmlAsync(reader).ConfigureAwait(false),
			nameof(MoveStringAction) => await MoveStringAction.ReadXmlAsync(reader).ConfigureAwait(false),
			nameof(RemoveAction) => await RemoveAction.ReadXmlAsync(reader).ConfigureAwait(false),
			nameof(ReplaceAction) => await ReplaceAction.ReadXmlAsync(reader).ConfigureAwait(false),
			nameof(ChangeRangeCaseAction) => await ChangeRangeCaseAction.ReadXmlAsync(reader).ConfigureAwait(false),
			nameof(ChangeStringCaseAction) => await ChangeStringCaseAction.ReadXmlAsync(reader).ConfigureAwait(false),

			_ => throw new XmlException($@"Unknown {nameof(IJobItem)} type: ""{reader.Name}""."),
		};
	}

	public static Task<IIndex> ReadIIndexAsync(this XmlReader reader)
	{
		return reader.Name switch
		{
			nameof(BeginningIndex) => BeginningIndex.ReadXmlAsync(reader),
			nameof(FileExtensionIndex) => FileExtensionIndex.ReadXmlAsync(reader),
			nameof(EndIndex) => EndIndex.ReadXmlAsync(reader),
			nameof(FixedIndex) => FixedIndex.ReadXmlAsync(reader),
			nameof(SubstringIndex) => SubstringIndex.ReadXmlAsync(reader),

			_ => throw new XmlException($@"Unknown {nameof(IIndex)} type: ""{reader.Name}""."),
		};
	}

	public static Task<IValueSource> ReadValueSourceAsync(this XmlReader reader)
	{
		return reader.Name switch
		{
			nameof(CounterValueSource) => CounterValueSource.ReadXmlAsync(reader),
			nameof(StringValueSource) => StringValueSource.ReadXmlAsync(reader),
			nameof(RandomStringValueSource) => RandomStringValueSource.ReadXmlAsync(reader),
			nameof(FolderNameValueSource) => FolderNameValueSource.ReadXmlAsync(reader),

			_ => throw new XmlException($@"Unknown {nameof(IValueSource)} type: ""{reader.Name}""."),
		};
	}

	public static Task<INumberFormatter> ReadNumberFormatterAsync(this XmlReader reader)
	{
		return reader.Name switch
		{
			nameof(PaddedNumberFormatter) => PaddedNumberFormatter.ReadXmlAsync(reader),
			nameof(RomanNumberFormatter) => RomanNumberFormatter.ReadXmlAsync(reader),
			nameof(NumberToWordsFormatter) => NumberToWordsFormatter.ReadXmlAsync(reader),

			_ => throw new XmlException($@"Unknown {nameof(IValueSource)} type: ""{reader.Name}""."),
		};
	}
}