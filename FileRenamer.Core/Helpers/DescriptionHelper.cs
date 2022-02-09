using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Helpers;

public static class DescriptionHelper
{
	public static string GetRangeFriendlyName(IIndex startIndex, IIndex endIndex)
	{
		return (startIndex, endIndex) switch
		{
			(BeginningIndex, FileExtensionIndex)
			 or (FixedIndex, FileExtensionIndex) when startIndex.FindIn(string.Empty) == 0 => "file name",
			(FileExtensionIndex, EndIndex) => "file extension",
			(BeginningIndex, EndIndex)
			 or (FixedIndex, EndIndex) when startIndex.FindIn(string.Empty) == 0 => "all characters",
			_ => $"characters from {startIndex.Description.ToString(includePreposition: false)} to {endIndex.Description.ToString(includePreposition: false)}",
		};
	}

}