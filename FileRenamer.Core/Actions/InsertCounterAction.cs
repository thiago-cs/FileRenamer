using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

public sealed class InsertCounterAction : RenameActionBase
{
	private readonly IIndexFinder insertIndexFinder;
	private readonly int startValue;
	private readonly int minWidth;
	private int counter;


	public InsertCounterAction(IIndexFinder insertIndexFinder, int startValue, int minWidth)
	{
		this.insertIndexFinder = insertIndexFinder ?? throw new ArgumentNullException(nameof(insertIndexFinder));
		this.startValue = startValue;
		this.minWidth = minWidth;

		Reset();
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int insertIndex = insertIndexFinder.FindIn(input);

		if (insertIndex < 0 || input.Length < insertIndex)
			return input;

		// 1.3. 
		string value = counter++.ToString().PadLeft(minWidth, '0');


		// 2. 
		return input.Insert(insertIndex, value);
	}

	public void Reset()
	{
		counter = startValue;
	}
}