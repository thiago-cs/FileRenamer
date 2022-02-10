using Humanizer;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Helpers;

public static class DescriptionHelper
{
	public static string GetRangeFriendlyName(IIndex startIndex, IIndex endIndex)
	{
		return (startIndex, endIndex) switch
		{
			(BeginningIndex, FileExtensionIndex) => "file name",
			(FixedIndex fix, FileExtensionIndex) when fix.index == 0 => "file name",

			(FileExtensionIndex, EndIndex) => "file extension",

			(BeginningIndex, EndIndex) => "all characters",
			(FixedIndex fix, EndIndex) when fix.index == 0 => "all characters",

			(FixedIndex fix, EndIndex) when fix.index == -1 => "the last character",
			(FixedIndex fix, EndIndex) when fix.index < 0 => "the last " + "character".ToQuantity(-fix.index),

			_ => $"characters from {startIndex.Description.ToString(includePreposition: false)} to {endIndex.Description.ToString(includePreposition: false)}",
		};
	}

}