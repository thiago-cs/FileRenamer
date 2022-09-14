using System.Xml;
using FileRenamer.Core.Extensions;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Serialization;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.Core.ValueSources;

public sealed class CounterValueSource : IValueSource
{
	#region Properties and fields

	public const int DefaultInitialValue = 1;
	public const int DefaultIncrement = 1;


	public string Description => $"a {Formatter?.Description ?? "counter"} ({GetValue(0)}, {GetValue(1)}, {GetValue(2)}, …)";

	/// <summary>
	/// Gets or sets the initial value of the counter.
	/// </summary>
	public int InitialValue { get; set; } = DefaultInitialValue;

	/// <summary>
	/// Gets or sets the .
	/// </summary>
	public int Increment { get; set; } = DefaultIncrement;

	/// <summary>
	/// Gets or sets the minimum number of characters in the resulting string.
	/// </summary>
	public INumberFormatter? Formatter { get; set; }

	#endregion


	public string GetValue(JobTarget target)
	{
		return GetValue(target.Index);
	}

	public string GetValue(int index)
	{
		int counter = InitialValue + index * Increment;

		string value = Formatter != null
					 ? Formatter.Format(counter)
					 : counter.ToString();

		return value;
	}


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(InitialValue), InitialValue).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(Increment), Increment).ConfigureAwait(false);

		if (Formatter != null)
			await writer.WriteElementAsync(nameof(Formatter), Formatter).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static async Task<IValueSource> ReadXmlAsync(XmlReader reader)
	{
		CounterValueSource counterValueSource = new();

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(InitialValue):
					counterValueSource.InitialValue = int.Parse(reader.Value);
					break;

				case nameof(Increment):
					counterValueSource.Increment = int.Parse(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(CounterValueSource));

		//
		while (reader.NodeType != XmlNodeType.EndElement)
			switch (reader.Name)
			{
				case nameof(Formatter):
					reader.ReadStartElement();
					counterValueSource.Formatter = await reader.ReadNumberFormatterAsync().ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				default:
					break;
			}

		// This element may or may not have descendant elements (i.e. Formatter).
		// In case there is no descendant elements, there is no end element.
		if (reader.Name == nameof(CounterValueSource))
			reader.ReadEndElement();

		return counterValueSource;
	}

	#endregion
}