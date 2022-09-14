using System.Text.RegularExpressions;
using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Indices;

public sealed class SubstringIndex : IIndex
{
	#region Properties and fields

	private Regex? regex;


	public string Value { get; set; }
	public bool Before { get; set; }
	public bool IgnoreCase { get; set; }
	public bool UseRegex { get; set; }
	public IndexDescription Description
	{
		get
		{
			System.Text.StringBuilder sb = new();

			sb.Append(Before ? "before " : "after ");

			if (UseRegex)
				sb.Append("the expression ");

			sb.Append('"')
			  .Append(Value)
			  .Append('"');

			if (IgnoreCase)
				sb.Append("(ignore case)");

			return new(null, sb.ToString());
		}
	}

	#endregion



	public SubstringIndex(string value, bool before, bool ignoreCase, bool useRegex)
	{
		this.Value = value;
		this.Before = before;
		this.IgnoreCase = ignoreCase;
		this.UseRegex = useRegex;
	}


	public int FindIn(string input)
	{
		// 1. 
		int index;

		if (UseRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (IgnoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(Value, regexOptions);
			}

			Match? match = regex.Match(input);

			index = match == null || !match.Success ? -1
				  : Before ? match.Index
				  : match.Index + match.Length;
		}
		else
		{
			index = input.IndexOf(Value, IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

			if (index != -1)
				index = Before ? index : index + Value.Length;
		}

		// 2. 
		return index;
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		writer.WriteStartElement(GetType().Name);

		await writer.WriteAttributeAsync(nameof(Value), Value).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(Before), Before).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IgnoreCase), IgnoreCase).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(UseRegex), UseRegex).ConfigureAwait(false);

		writer.WriteEndElement();
	}

	public static Task<IIndex> ReadXmlAsync(XmlReader reader)
	{
		string? value = null;
		bool? before = null;
		bool? ignoreCase = null;
		bool? useRegex = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(Value):
					value = reader.Value;
					break;

				case nameof(Before):
					before = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(IgnoreCase):
					ignoreCase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(UseRegex):
					useRegex = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(SubstringIndex));

		//
		XmlSerializationHelper.ThrowIfNull(value, nameof(Value));
		XmlSerializationHelper.ThrowIfNull(before, nameof(Before));
		XmlSerializationHelper.ThrowIfNull(ignoreCase, nameof(IgnoreCase));
		XmlSerializationHelper.ThrowIfNull(useRegex, nameof(UseRegex));

		SubstringIndex result = new(value, before.Value, ignoreCase.Value, useRegex.Value);
		return Task.FromResult(result as IIndex);
	}

	#endregion
}