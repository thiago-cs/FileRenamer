using FileRenamer.Core.Indices;


namespace FileRenamer.Core.Helpers;

public static class DescriptionHelper
{
	public static string GetRangeFriendlyName(IIndex startIndex, IIndex endIndex)
	{
		return (startIndex, endIndex) switch
		{
			(BeginningIndex, FileExtensionIndex) => $"file name",
			(FileExtensionIndex, EndIndex) => $"file extension",
			(BeginningIndex, EndIndex) => $"all characters",
			_ => $"characters from {startIndex.Description.ToString(includePreposition: false)} to {endIndex.Description.ToString(includePreposition: false)}",
		};
	}

}