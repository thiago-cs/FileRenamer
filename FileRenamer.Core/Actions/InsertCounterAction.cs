using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class InsertCounterAction : RenameActionBase
{
	private readonly IIndex insertIndex;
	private readonly int startValue;
	private readonly int minWidth;
	private int counter;


	public InsertCounterAction(IIndex insertIndex, int startValue, int minWidth)
	{
		this.insertIndex = insertIndex ?? throw new ArgumentNullException(nameof(insertIndex));
		this.startValue = startValue;
		this.minWidth = minWidth;

		UpdateDescription();
		Reset();
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int index = insertIndex.FindIn(input);

		if (index < 0 || input.Length < index)
			return input;

		// 1.3. 
		string value = counter++.ToString().PadLeft(minWidth, '0');


		// 2. 
		return input.Insert(index, value);
	}

	public override void UpdateDescription()
	{
		Description = $"insert a {minWidth}-char counter starting from {startValue} {insertIndex.Description.ToString(includePreposition: true)}";
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return new InsertCounterAction(insertIndex, startValue, minWidth);
	}

	public void Reset()
	{
		counter = startValue;
	}
}