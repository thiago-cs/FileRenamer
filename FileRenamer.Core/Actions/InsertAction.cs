using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class InsertAction : RenameActionBase
{
	private readonly IIndexFinder insertIndexFinder;
	private readonly string value;


	public InsertAction(IIndexFinder insertIndexFinder, string value)
	{
		this.insertIndexFinder = insertIndexFinder ?? throw new ArgumentNullException(nameof(insertIndexFinder));
		this.value = value ?? throw new ArgumentNullException(nameof(value));

		Description = @$"insert ""{this.value}"" {this.insertIndexFinder.Description.ToString(includePreposition: true)}";
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


		// 2. 
		return input.Insert(insertIndex, value);
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return new InsertAction(insertIndexFinder, value);
	}
}