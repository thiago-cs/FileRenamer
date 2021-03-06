using System.Text.RegularExpressions;


namespace FileRenamer.Core.Indices;

public sealed class SubstringIndex : IIndex
{
	private readonly string value;
	private readonly bool before;
	private readonly bool ignoreCase;
	private readonly bool useRegex;
	private Regex? regex;


	public IndexDescription Description
	{
		get
		{
			System.Text.StringBuilder sb = new();

			sb.Append(before ? "before " : "after ");

			if (useRegex)
				sb.Append("the expression ");

			sb.Append('"')
			  .Append(value)
			  .Append('"');

			if (ignoreCase)
				sb.Append("(ignore case)");

			return new(null, sb.ToString());
		}
	}



	public SubstringIndex(string value, bool before, bool ignoreCase, bool useRegex)
	{
		this.value = value;
		this.before = before;
		this.ignoreCase = ignoreCase;
		this.useRegex = useRegex;
	}


	public int FindIn(string input)
	{
		// 1. 
		int index;

		if (useRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (ignoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(value, regexOptions);
			}

			Match? match = regex.Match(input);

			index = match == null || !match.Success ? -1
				  : before ? match.Index
				  : match.Index + match.Length;
		}
		else
		{
			index = input.IndexOf(value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

			if (index != -1)
				index = before ? index : index + value.Length;
		}

		// 2. 
		return index;
	}
}