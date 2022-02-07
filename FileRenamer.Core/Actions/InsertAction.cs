using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class InsertAction : RenameActionBase
{
	private readonly IIndex insertIndex;
	private readonly string value;


	public InsertAction(IIndex insertIndex, string value)
	{
		this.insertIndex = insertIndex ?? throw new ArgumentNullException(nameof(insertIndex));
		this.value = value ?? throw new ArgumentNullException(nameof(value));

		UpdateDescription();
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


		// 2. 
		return input.Insert(index, value);
	}

	public override void UpdateDescription()
	{
		Description = @$"insert ""{value}"" {insertIndex.Description.ToString(includePreposition: true)}";
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return new InsertAction(insertIndex, value);
	}
}