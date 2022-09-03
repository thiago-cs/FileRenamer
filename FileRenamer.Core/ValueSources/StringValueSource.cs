using FileRenamer.Core.Jobs;


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
}