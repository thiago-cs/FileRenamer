using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Actions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class RemoveAction : RenameActionBase
{
	private readonly IIndex startIndex;
	private readonly IIndex? endIndex;
	private readonly int count;


	public RemoveAction(IIndex startIndex, IIndex endIndex)
	{
		this.startIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		this.endIndex = endIndex ?? throw new ArgumentNullException(nameof(endIndex));

		UpdateDescription();
	}

	public RemoveAction(IIndex startIndex, int count)
	{
		this.startIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		this.count = count != 0 ? count : throw new ArgumentOutOfRangeException(nameof(count), "The number of characters to remove may not be zero.");

		UpdateDescription();
	}


	public override string Run(string input)
	{
		// 1. 
		// 1.1. 
		if (!IsEnabled)
			return input;

		// 1.2. 
		int startIndex = this.startIndex.FindIn(input);

		if (startIndex < 0 || input.Length < startIndex)
			return input;

		// 1.3. 
		int endIndex;

		if (this.endIndex == null)
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
			endIndex = this.endIndex.FindIn(input);

			if (endIndex < startIndex)
				return input;
		}


		// 2. 
		string result = input[0..startIndex];

		return endIndex < input.Length
				? result + input[endIndex..]
				: result;
	}

	public override void UpdateDescription()
	{
		if (endIndex != null)
		{
			Description = $"remove {Helpers.DescriptionHelper.GetRangeFriendlyName(startIndex, endIndex)}";
		}
		else
		{
			string s = count switch
			{
				>= 2 => $"{count} characters",
				1 => "a character",
				-1 => "character",
				_ => $"{-count} characters",
			};

			Description = 0 < count
				? $"remove {s} after the {startIndex.Description.ToString(includePreposition: false)}"
				: startIndex is EndIndex
					? $"remove the last {s}"
					: $"remove the {s} preceding the {startIndex.Description.ToString(includePreposition: false)}";
		}
	}

	/// <inheritdoc cref="RenameActionBase.Clone" />
	public override RenameActionBase Clone()
	{
		return endIndex != null
				? new RemoveAction(startIndex, endIndex)
				: new RemoveAction(startIndex, count);
	}
}