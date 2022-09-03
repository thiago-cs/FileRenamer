using Humanizer;


namespace FileRenamer.Core.Indices;

public sealed class FixedIndex : IIndex
{
	public int Index { get; set; }


	public IndexDescription Description
	{
		get
		{
			const string startPrepositon = "after the";

			return Index switch
			{
				< -1 => new(startPrepositon, $"{(-Index).Ordinalize(GrammaticalGender.Neuter)} to last character"),
				-1 => new(startPrepositon, "last character"),
				0 => new(startPrepositon, "1st character"),
				_ => new("after the", $"{Index.Ordinalize(GrammaticalGender.Neuter)} character")
			};
		}
	}


	public FixedIndex(int value)
	{
		Index = value;
	}

	public static implicit operator FixedIndex(int value) => new(value);


	public int FindIn(string input)
	{
		return Index < 0 ? input.Length + Index : Index;
	}
}