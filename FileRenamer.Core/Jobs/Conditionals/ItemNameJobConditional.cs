using System.Text.RegularExpressions;
using System.Xml;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs.Conditionals;

public sealed class ItemNameJobConditional : ConditionalJobItem
{
	private Regex? regex;

	public string Pattern { get; }
	public bool IgnoreCase { get; }


	#region Constructors

	public ItemNameJobConditional(string pattern, bool ignoreCase)
		: this(pattern, ignoreCase, new())
	{ }

	public ItemNameJobConditional(string pattern, bool ignoreCase, JobCollection jobs)
		: base(jobs)
	{
		if (string.IsNullOrEmpty(pattern))
			throw new Exception($@"""{nameof(Pattern)}"" cannot be empty.");

		Pattern = pattern;
		IgnoreCase = ignoreCase;

		UpdateDescription();
	}

	#endregion


	private void UpdateDescription()
	{
		System.Text.StringBuilder sb = new();

		sb.Append(@"if name matches """)
		  .Append(Pattern)
		  .Append('"');

		if (IgnoreCase)
			sb.Append(" (ignore case)");

		Description = sb.ToString();
	}

	public override bool TestTarget(JobTarget target)
	{
		if (regex == null)
		{
			RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

			if (IgnoreCase)
				regexOptions |= RegexOptions.IgnoreCase;

			regex = new(Pattern, regexOptions);
		}

		return regex.IsMatch(target.NewFileName);
	}

	public override void Run(JobTarget target, JobContext context)
	{
		if (TestTarget(target))
			Jobs.Run(target, context);
	}

	public override JobItem DeepCopy()
	{
		return new ItemNameJobConditional(Pattern, IgnoreCase, Jobs.DeepCopy());
	}


	#region XML serialization

	public override async Task WriteXmlAsync(XmlWriter writer)
	{
		writer.WriteStartElement(GetType().Name);

		await writer.WriteAttributeAsync(nameof(Pattern), Pattern).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IgnoreCase), IgnoreCase).ConfigureAwait(false);

		await writer.WriteElementAsync(nameof(Jobs), Jobs).ConfigureAwait(false);

		writer.WriteEndElement();
	}

	public static async Task<ItemNameJobConditional> ReadXmlAsync(XmlReader reader)
	{
		bool isEnable = true;
		string? pattern = null;
		bool? ignoreCase = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(IsEnabled):
					isEnable = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(Pattern):
					pattern = reader.Value;
					break;

				case nameof(IgnoreCase):
					ignoreCase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(ItemNameJobConditional));

		//
		JobCollection? jobs = null;

		while (reader.NodeType != XmlNodeType.EndElement)
			switch (reader.Name)
			{
				case nameof(Jobs):
					reader.ReadStartElement();
					jobs = await JobCollection.ReadXmlAsync(reader).ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				default:
					break;
			}

		reader.ReadEndElement();

		//
		XmlSerializationHelper.ThrowIfNull(pattern, nameof(Pattern));
		XmlSerializationHelper.ThrowIfNull(ignoreCase, nameof(IgnoreCase));

		ItemNameJobConditional result = new(pattern, ignoreCase.Value, jobs ?? new()) { IsEnabled = isEnable };
		return result;
	}

	#endregion
}