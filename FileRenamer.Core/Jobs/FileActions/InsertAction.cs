using System.Xml;
using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Serialization;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class InsertAction : RenameFileJob
{
	#region Properties and fields

	public IIndex InsertIndex { get; }
	public IValueSource ValueSource { get; }
	
	#endregion


	public InsertAction(IIndex insertIndex, IValueSource value)
	{
		InsertIndex = insertIndex ?? throw new ArgumentNullException(nameof(insertIndex));
		ValueSource = value ?? throw new ArgumentNullException(nameof(value));

		UpdateDescription();
	}


	#region RenameActionBase implementation

	public override void Run(JobTarget target, JobContext context)
	{
		string input = target.NewFileName;

		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return;

		// 1.2. 
		int index = InsertIndex.FindIn(input);

		if (index < 0 || input.Length < index)
			return;


		// 2. 
		target.NewFileName = input.Insert(index, ValueSource.GetValue(target));
		return;
	}

	public override void UpdateDescription()
	{
		Description = @$"insert {ValueSource.Description} {InsertIndex.Description.ToString(includePreposition: true)}";
	}

	public override RenameFileJob DeepCopy()
	{
		return new InsertAction(InsertIndex, ValueSource);
	}

	#endregion


	#region XML serialization

	public override async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(IsEnabled), IsEnabled).ConfigureAwait(false);

		await writer.WriteElementAsync(nameof(InsertIndex), InsertIndex).ConfigureAwait(false);
		await writer.WriteElementAsync(nameof(ValueSource), ValueSource).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static async Task<RenameFileJob> ReadXmlAsync(XmlReader reader)
	{
		//
		bool isEnable = true;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(IsEnabled):
					isEnable = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(InsertAction));

		//
		IIndex? insertIndex = null;
		IValueSource? value = null;

		while (reader.NodeType != XmlNodeType.EndElement)
			switch (reader.Name)
			{
				case nameof(InsertIndex):
					reader.ReadStartElement();
					insertIndex = await reader.ReadIIndexAsync().ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				case nameof(ValueSource):
					reader.ReadStartElement();
					value = await reader.ReadValueSourceAsync().ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				default:
					break;
			}

		reader.ReadEndElement();

		//
		XmlSerializationHelper.ThrowIfNull(insertIndex, nameof(InsertIndex));
		XmlSerializationHelper.ThrowIfNull(value, nameof(ValueSource));

		return new InsertAction(insertIndex, value) { IsEnabled = isEnable };
	}

	#endregion
}