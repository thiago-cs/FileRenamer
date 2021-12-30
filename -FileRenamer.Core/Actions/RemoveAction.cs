using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

public class RemoveAction : RenameActionBase
{
	private IIndexFinder _startIndexFinder;
	public IIndexFinder StartIndexFinder { get => _startIndexFinder; set => SetProperty(ref _startIndexFinder, value); }

	private IIndexFinder _endIndexFinder;
	public IIndexFinder EndIndexFinder { get => _endIndexFinder; set => SetProperty(ref _endIndexFinder, value); }


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int startIndex = StartIndexFinder.FindIndex(input);

		if (startIndex < 0 || input.Length <= startIndex)
			return input;

		// 1.3. 
		int endIndex = EndIndexFinder.FindIndex(input);

		if (endIndex < startIndex)
			return input;

		if (input.Length <= endIndex)
			endIndex = startIndex;

		// 2. 
		return input[..(startIndex + 1)] + input[endIndex..];
	}
}