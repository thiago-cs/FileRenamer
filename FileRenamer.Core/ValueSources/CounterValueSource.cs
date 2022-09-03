using FileRenamer.Core.Jobs;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.Core.ValueSources;

public sealed class CounterValueSource : IValueSource
{
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
}