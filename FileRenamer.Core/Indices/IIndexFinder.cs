﻿namespace FileRenamer.Core.Indices;

/// <summary>
/// Represents an object that provides methods for finding an index within a <see cref="string"/>.
/// </summary>
public interface IIndexFinder
{
	/// <summary>
	/// Gets this objects description to be used to compose a <see cref="Actions.RenameActionBase"/>'s own description.
	/// </summary>
	public IndexFinderDescription Description { get; }


	/// <summary>
	/// Finds an index within the specified string according to the logic implemented in a derived class.
	/// </summary>
	/// <param name="input">A <see cref="string"/>.</param>
	/// <returns>An index within the specified string.</returns>
	internal int FindIn(string input);
}