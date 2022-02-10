using Humanizer;


namespace FileRenamer.Core.Indices;

public sealed class FixedIndex : IIndex
{
	internal readonly int index;


	public IndexDescription Description
	{
		get
		{
			const string startPrepositon = "before the";

			return index switch
			{
				< -1 => new(startPrepositon, $"{(-index).Ordinalize(GrammaticalGender.Neuter)} to last character"),
				-1 => new(startPrepositon, "last character"),
				0 => new(startPrepositon, "1st character"),
				_ => new("after the", $"{index.Ordinalize(GrammaticalGender.Neuter)} character")
			};
		}
	}


	public FixedIndex(int value)
	{
		index = value;
	}

	public static implicit operator FixedIndex(int value) => new(value);


	public int FindIn(string input)
	{
		return index < 0 ? input.Length + index : index;
	}
}