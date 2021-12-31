using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

public sealed class ToCaseAction : RenameActionBase
{
	private readonly IIndexFinder startIndexFinder;
	private readonly IIndexFinder endIndexFinder;
	private readonly TextCasing textCase;
	//public TextCase Case { get; set; }


	public ToCaseAction(IIndexFinder startIndexFinder, IIndexFinder endIndexFinder, TextCasing textCase)
	{
		this.textCase = textCase;
		this.endIndexFinder = endIndexFinder;
		this.startIndexFinder = startIndexFinder;
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int startIndex = startIndexFinder.FindIn(input);

		if (startIndex < 0 || input.Length <= startIndex)
			return input;

		// 1.3. 
		int endIndex = endIndexFinder.FindIn(input);

		if (endIndex < startIndex)
			return input;

		// 2. 
		string s1 = input[..startIndex];
		string s2 = input[startIndex..endIndex].ToCase(textCase);
		string s3 = input[endIndex..];

		// 3. 
		return s1 + s2 + s3;
	}
}