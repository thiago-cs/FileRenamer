using System.Xml;
using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ChangeRangeCaseAction : RenameActionBase
{
	#region Properties and fields

	public IIndex StartIndex { get; set; }
	public IIndex EndIndex { get; set; }
	public TextCasing TextCase { get; }
	
	#endregion


	public ChangeRangeCaseAction(IIndex startIndex, IIndex endIndex, TextCasing textCase)
	{
		StartIndex = startIndex;
		EndIndex = endIndex;
		TextCase = textCase;

		UpdateDescription();
	}


	#region RenameActionBase implementation

	public override void Run(JobTarget target, JobContext context)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return;

		// 1.2. 
		string input = target.NewFileName;

		int startIndex = StartIndex.FindIn(input);

		if (startIndex < 0 || input.Length <= startIndex)
			return;

		// 1.3. 
		int endIndex = EndIndex.FindIn(input);

		if (endIndex < startIndex)
			return;

		// 2. 
		string s1 = input[..startIndex];
		string s2 = input[startIndex..endIndex].ToCase(TextCase);
		string s3 = input[endIndex..];

		// 3. 
		target.NewFileName = s1 + s2 + s3;
	}

	public override void UpdateDescription()
	{
		string @case = TextCase switch
		{
			TextCasing.LowerCase => "lowercase",
			TextCasing.UpperCase => "uppercase",
			TextCasing.SentenceCase => "sentence case",
			TextCasing.TitleCase => "title case",
			TextCasing.TitleCaseIgnoreCommonWords => "title case (ignore common words)",
			_ => TextCase.ToString(),
		};

		Description = $"change {Indices.Range.GetDescription(StartIndex, EndIndex)} to {@case}";
	}

	public override RenameActionBase DeepCopy()
	{
		return new ChangeRangeCaseAction(StartIndex, EndIndex, TextCase);
	}

	#endregion


	#region XML serialization

	public override async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(TextCase), TextCase).ConfigureAwait(false);
		await writer.WriteElementAsync(nameof(StartIndex), StartIndex).ConfigureAwait(false);
		await writer.WriteElementAsync(nameof(EndIndex), EndIndex).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static async Task<RenameActionBase> ReadXmlAsync(XmlReader reader)
	{
		//
		TextCasing? textCase = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(TextCase):
					textCase = Enum.Parse<TextCasing>(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(ChangeRangeCaseAction));

		//
		IIndex? startIndex = null;
		IIndex? endIndex = null;

		while (reader.NodeType != XmlNodeType.EndElement)
			switch (reader.Name)
			{
				case nameof(StartIndex):
					reader.ReadStartElement();
					startIndex = await reader.ReadIIndexAsync().ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				case nameof(EndIndex):
					reader.ReadStartElement();
					endIndex = await reader.ReadIIndexAsync().ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				default:
					throw new XmlException($@"Unknown property ""{reader.Name}"".");
			}

		reader.ReadEndElement();

		//
		XmlSerializationHelper.ThrowIfNull(startIndex, nameof(StartIndex));
		XmlSerializationHelper.ThrowIfNull(endIndex, nameof(EndIndex));
		XmlSerializationHelper.ThrowIfNull(textCase, nameof(TextCase));

		return new ChangeRangeCaseAction(startIndex, endIndex, textCase.Value);
	}

	#endregion
}