using System.Text.RegularExpressions;


namespace FileRenamer.Core.Indices;

public sealed class SubstringIndex : IIndex
{
	private Regex? regex;


	public string Value {get; set; }
	public bool Before {get; set; }
	public bool IgnoreCase {get; set; }
	public bool UseRegex { get; set; }
	public IndexDescription Description
	{
		get
		{
			System.Text.StringBuilder sb = new();

			sb.Append(Before ? "before " : "after ");

			if (UseRegex)
				sb.Append("the expression ");

			sb.Append('"')
			  .Append(Value)
			  .Append('"');

			if (IgnoreCase)
				sb.Append("(ignore case)");

			return new(null, sb.ToString());
		}
	}



	public SubstringIndex(string value, bool before, bool ignoreCase, bool useRegex)
	{
		this.Value = value;
		this.Before = before;
		this.IgnoreCase = ignoreCase;
		this.UseRegex = useRegex;
	}


	public int FindIn(string input)
	{
		// 1. 
		int index;

		if (UseRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (IgnoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(Value, regexOptions);
			}

			Match? match = regex.Match(input);

			index = match == null || !match.Success ? -1
				  : Before ? match.Index
				  : match.Index + match.Length;
		}
		else
		{
			index = input.IndexOf(Value, IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

			if (index != -1)
				index = Before ? index : index + Value.Length;
		}

		// 2. 
		return index;
	}
}