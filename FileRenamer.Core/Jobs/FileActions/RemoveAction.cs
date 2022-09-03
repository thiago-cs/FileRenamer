using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class RemoveAction : RenameActionBase
{
	public IIndex StartIndex { get; set; }
	public IIndex? EndIndex { get; set; }
	public int Count { get; set; }


	public RemoveAction(IIndex startIndex, IIndex endIndex)
	{
		StartIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		EndIndex = endIndex ?? throw new ArgumentNullException(nameof(endIndex));

		UpdateDescription();
	}

	public RemoveAction(IIndex startIndex, int count)
	{
		StartIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		Count = count != 0 ? count : throw new ArgumentOutOfRangeException(nameof(count), "The number of characters to remove may not be zero.");

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
		int startIndex = StartIndex.FindIn(input);

		if (startIndex < 0 || input.Length < startIndex)
			return;

		// 1.3. 
		int endIndex;

		if (EndIndex == null)
		{
			if (Count < 0)
			{
				endIndex = startIndex;
				startIndex += Count;
			}
			else
				endIndex = startIndex <= int.MaxValue - Count
							? startIndex + Count
							: int.MaxValue;
		}
		else
		{
			endIndex = EndIndex.FindIn(input);

			if (endIndex < startIndex)
				return;
		}


		// 2. 
		string result = input[0..startIndex];

		target.NewFileName = endIndex < input.Length ? result + input[endIndex..] : result;
		return;
	}

	public override void UpdateDescription()
	{
		if (EndIndex != null)
		{
			Description = $"remove {Indices.Range.GetDescription(StartIndex, EndIndex)}";
		}
		else
		{
			string s = Count switch
			{
				>= 2 => $"{Count} characters",
				1 => "a character",
				-1 => "character",
				_ => $"{-Count} characters",
			};

			Description = 0 < Count
				? StartIndex is FixedIndex fix && -fix.Index == Count
					? $"remove the last {s}".Replace(" a ", null) // a small hack to avoid more conditionals.
					: $"remove {s} {StartIndex.Description.ToString(includePreposition: true)}"
				: StartIndex is EndIndex
					? $"remove the last {s}"
					: $"remove the {s} preceding the {StartIndex.Description.ToString(includePreposition: false)}";
		}
	}

	public override RenameActionBase DeepCopy()
	{
		return EndIndex != null
				? new RemoveAction(StartIndex, EndIndex)
				: new RemoveAction(StartIndex, Count);
	}
}