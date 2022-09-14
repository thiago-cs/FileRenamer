using System.Xml;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.ValueSources;

public sealed class StringValueSource : IValueSource
{
	public string Description => $"\"{Value}\"";

	/// <summary>
	/// Gets or sets the string value provided on every request.
	/// </summary>
	public string Value { get; set; } = "";


	public string GetValue(JobTarget target)
	{
		return Value;
	}


	public static implicit operator StringValueSource(string s) => new() { Value = s };


	#region XML serialization

	public async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(Value), Value).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<IValueSource> ReadXmlAsync(XmlReader reader)
	{
		string? value = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(Value):
					value = reader.Value;
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(StringValueSource));

		//
		XmlSerializationHelper.ThrowIfNull(value, nameof(Value));

		StringValueSource result = new StringValueSource() { Value = value };
		return Task.FromResult(result as IValueSource);
	}

	#endregion
}