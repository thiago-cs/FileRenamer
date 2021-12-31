using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

public sealed class RemoveAction : RenameActionBase
{
	private readonly IIndexFinder startIndexFinder;
	private readonly IIndexFinder? endIndexFinder;
	private readonly int count;


	public RemoveAction(IIndexFinder startIndexFinder, IIndexFinder endIndexFinder)
	{
		this.startIndexFinder = startIndexFinder ?? throw new ArgumentNullException(nameof(startIndexFinder));
		this.endIndexFinder = endIndexFinder ?? throw new ArgumentNullException(nameof(endIndexFinder));
	}

	public RemoveAction(IIndexFinder startIndexFinder, int count)
	{
		this.startIndexFinder = startIndexFinder ?? throw new ArgumentNullException(nameof(startIndexFinder));
		this.count = count;
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int startIndex = startIndexFinder.FindIn(input);

		if (startIndex < 0 || input.Length < startIndex)
			return input;

		// 1.3. 
		int endIndex;

		if (endIndexFinder == null)
		{
			if (count < 0)
			{
				endIndex = startIndex;
				startIndex += count;
			}
			else
				endIndex = startIndex <= int.MaxValue - count
					? startIndex + count
					: int.MaxValue;
		}
		else
		{
			endIndex = endIndexFinder.FindIn(input);

			if (endIndex < startIndex)
				return input;
		}


		// 2. 
		string result = input[0..startIndex];

		return endIndex < input.Length
				? result + input[endIndex..]
				: result;
	}
}