﻿using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Helpers;

public static class DescriptionHelper
{
	public static string GetRangeFriendlyName(IIndex startIndexFinder, IIndex endIndexFinder)
	{
		return (startIndexFinder, endIndexFinder) switch
		{
			(BeginningIndex, FileExtensionIndexFinder) => $"file name",
			(FileExtensionIndexFinder, EndIndexFinder) => $"file extension",
			(BeginningIndex, EndIndexFinder) => $"all characters",
			_ => $"characters from {startIndexFinder.Description.ToString(includePreposition: false)} to {endIndexFinder.Description.ToString(includePreposition: false)}",
		};
	}

}