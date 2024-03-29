﻿using System.Xml;
using Humanizer;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Indices;

public sealed class FixedIndex : IIndex
{
	public int Index { get; }


	public IndexDescription Description
	{
		get
		{
			const string textBefore = "before the";
			const string textAfter = "after the";

			return Index switch
			{
				< -2 => new(textBefore, $"{(-Index - 1).Ordinalize(GrammaticalGender.Neuter)} to last character"),
				-2 => new(textBefore, "last character"),
				-1 => new(textAfter, "last character"),
				0 => new(textBefore, "1st character"),
				_ => new(textAfter, $"{Index.Ordinalize(GrammaticalGender.Neuter)} character")
			};
		}
	}


	public FixedIndex(int index)
	{
		Index = index;
	}

	public static implicit operator FixedIndex(int index) => new(index);


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