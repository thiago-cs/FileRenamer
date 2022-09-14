namespace FileRenamer.Core.ValueSources.NumberFormatters;

public interface INumberFormatter : Serialization.IXmlSerializableAsync
{
	/// <summary>
	/// Gets the description for the value that is provided by this <see cref="INumberFormatter"/> instance.
	/// </summary>
	string Description { get; }

	string Format(int n);
}