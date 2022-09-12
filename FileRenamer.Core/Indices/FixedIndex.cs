using System.Xml;
using Humanizer;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Indices;

public sealed class FixedIndex : IIndex
{
	public int Index { get; set; }


	public IndexDescription Description
	{
		get
		{
			const string startPrepositon = "after the";

			return Index switch
			{
				< -1 => new(startPrepositon, $"{(-Index).Ordinalize(GrammaticalGender.Neuter)} to last character"),
				-1 => new(startPrepositon, "last character"),
				0 => new(startPrepositon, "1st character"),
				_ => new("after the", $"{Index.Ordinalize(GrammaticalGender.Neuter)} character")
			};
		}
	}


	public FixedIndex(int value)
	{
		Index = value;
	}

	public static implicit operator FixedIndex(int value) => new(value);


	public int FindIn(string input)
	{
		return Index < 0 ? input.Length + Index : Index;
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(Index), Index).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IIndex> ReadXmlAsync(XmlReader reader)
	{
		int? index = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(Index):
					index = int.Parse(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(FixedIndex));

		//
		XmlSerializationHelper.ThrowIfNull(index, nameof(Index));

		return Task.FromResult(new FixedIndex(index.Value) as IIndex);
	}

	#endregion
}