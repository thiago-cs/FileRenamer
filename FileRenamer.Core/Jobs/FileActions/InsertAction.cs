using FileRenamer.Core.Indices;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class InsertAction : RenameActionBase
{
	public IIndex InsertIndex { get; }
	public IValueSource ValueSource { get; }


	public InsertAction(IIndex insertIndex, IValueSource value)
	{
		InsertIndex = insertIndex ?? throw new ArgumentNullException(nameof(insertIndex));
		ValueSource = value ?? throw new ArgumentNullException(nameof(value));

		UpdateDescription();
	}


	public override void Run(JobTarget target, JobContext context)
	{
		string input = target.NewFileName;

		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return;

		// 1.2. 
		int index = InsertIndex.FindIn(input);

		if (index < 0 || input.Length < index)
			return;


		// 2. 
		target.NewFileName = input.Insert(index, ValueSource.GetValue(target));
		return;
	}

	public override void UpdateDescription()
	{
		Description = @$"insert {ValueSource.Description} {InsertIndex.Description.ToString(includePreposition: true)}";
	}

	public override RenameActionBase DeepCopy()
	{
		return new InsertAction(InsertIndex, ValueSource);
	}
}