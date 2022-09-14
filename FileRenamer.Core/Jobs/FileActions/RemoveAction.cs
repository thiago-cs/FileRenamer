using System.Xml;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class RemoveAction : RenameActionBase
{
	#region Properties and fields

	public IIndex StartIndex { get; }
	public IIndex? EndIndex { get; }
	public int Count { get; }

	#endregion


	public RemoveAction(IIndex startIndex, IIndex endIndex)
	{
		StartIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		EndIndex = endIndex ?? throw new ArgumentNullException(nameof(endIndex));

		UpdateDescription();
	}

	public RemoveAction(IIndex startIndex, int count)
	{
		StartIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		Count = count != 0 ? count : throw new ArgumentOutOfRangeException(nameof(count), "The number of characters to remove may not be zero.");

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
		int startIndex = StartIndex.FindIn(input);

		if (startIndex < 0 || input.Length < startIndex)
			return;

		// 1.3. 
		int endIndex;

		if (EndIndex == null)
		{
			if (Count < 0)
			{
				endIndex = startIndex;
				startIndex += Count;
			}
			else
				endIndex = startIndex <= int.MaxValue - Count
							? startIndex + Count
							: int.MaxValue;
		}
		else
		{
			endIndex = EndIndex.FindIn(input);

			if (endIndex < startIndex)
				return;
		}


		// 2. 
		string result = input[0..startIndex];

		target.NewFileName = endIndex < input.Length ? result + input[endIndex..] : result;
		return;
	}

	public override void UpdateDescription()
	{
		if (EndIndex != null)
		{
			Description = $"remove {Indices.Range.GetDescription(StartIndex, EndIndex)}";
		}
		else
		{
			string s = Count switch
			{
				>= 2 => $"{Count} characters",
				1 => "a character",
				-1 => "character",
				_ => $"{-Count} characters",
			};

			Description = 0 < Count
				? StartIndex is FixedIndex fix && -fix.Index == Count
					? $"remove the last {s}".Replace(" a ", null) // a small hack to avoid more conditionals.
					: $"remove {s} {StartIndex.Description.ToString(includePreposition: true)}"
				: StartIndex is EndIndex
					? $"remove the last {s}"
					: $"remove the {s} preceding the {StartIndex.Description.ToString(includePreposition: false)}";
		}
	}

	public override RenameActionBase DeepCopy()
	{
		return EndIndex != null
				? new RemoveAction(StartIndex, EndIndex)
				: new RemoveAction(StartIndex, Count);
	}

	#endregion


	#region XML serialization

	public override async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		if (EndIndex == null)
			await writer.WriteAttributeAsync(nameof(Count), Count).ConfigureAwait(false);

		await writer.WriteElementAsync(nameof(StartIndex), StartIndex).ConfigureAwait(false);

		if (EndIndex != null)
			await writer.WriteElementAsync(nameof(EndIndex), EndIndex).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static async Task<RenameActionBase> ReadXmlAsync(XmlReader reader)
	{
		//
		int? count = null;

		if (reader.AttributeCount != 0 && reader.GetAttribute(nameof(Count)) is string value)
			count = int.Parse(value);

		reader.ReadStartElement(nameof(RemoveAction));

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

		if (count == null && endIndex == null)
			throw new XmlException($"A value for either the {nameof(Count)} or the {nameof(EndIndex)} property must be specified in XML.");

		if (count != null && endIndex != null)
			throw new XmlException($"Values for both the {nameof(Count)} and the {nameof(EndIndex)} properties were specified in XML.");

		return count != null
			 ? new RemoveAction(startIndex, count.Value)
			 : new RemoveAction(startIndex, endIndex!);
	}

	#endregion
}