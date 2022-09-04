using System.Text.RegularExpressions;
using Humanizer;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class MoveStringAction : RenameActionBase
{
	private Regex? regex;

	public string Text { get; set; }
	public bool IgnoreCase { get; set; }
	public bool UseRegex { get; set; }
	public int Count { get; set; }


	public MoveStringAction(string text, bool ignoreCase, bool useRegex, int length)
	{
		Text = text ?? throw new ArgumentNullException(nameof(text));
		IgnoreCase = ignoreCase;
		UseRegex = useRegex;
		Count = length;

		UpdateDescription();
	}


	public override void Run(JobTarget target, JobContext context)
	{
		// 0. 
		ArgumentNullException.ThrowIfNull(Text, nameof(Text));

		if (Text.Length == 0)
			throw new InvalidOperationException($"{nameof(Text)} cannot be empty.");

		if (Count == 0)
			return;

		// 1.
		string input = target.NewFileName;
		string value;
		int index;

		// 1.1.
		if (UseRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (IgnoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(Text, regexOptions);
			}

			Match match = regex.Match(input);

			if (!match.Success || match.Value.Length == 0)
				return;

			value = match.Value;
			index = match.Index;
		}
		// 1.2.
		else
		{
			value = Text;
			index = input.IndexOf(Text, IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

			if (index == -1)
				return;
		}

		// 2. 
		int endIndex = index + value.Length;
		int min = -index;
		int max = input.Length - endIndex;

		int count = Count < min ? min
				  : max < Count ? max
				  : Count;

		target.NewFileName = (input[..index] + input[endIndex..]).Insert(index + count, value);
	}

	public override void UpdateDescription()
	{
		int count = Math.Abs(Count);

		System.Text.StringBuilder sb = new("move ");

		if (UseRegex)
			sb.Append("the expression ");

		sb.Append('"')
		  .Append(Text)
		  .Append(@""" ")
		  .Append("position".ToQuantity(count))
		  .Append(" to the ")
		  .Append(Count < 0 ? "left" : "right");

		if (IgnoreCase)
			sb.Append(" (ignore case)");

		Description = sb.ToString();
	}

	public override RenameActionBase DeepCopy()
	{
		return new MoveStringAction(Text, IgnoreCase, UseRegex, Count);
	}
}