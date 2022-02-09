﻿namespace FileRenamer.Core.Indices;

public sealed class FixedIndex : IIndex
{
	private readonly int index;


	public IndexDescription Description => new("after", $"char. #{index}");


	public FixedIndex(int value)
	{
		index = value;
	}

	public static implicit operator FixedIndex(int value) => new(value);


	public int FindIn(string input)
	{
		return index;
	}
}