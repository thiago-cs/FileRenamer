using Humanizer;


namespace FileRenamer.Core.Indices;

public static class Range
{
	public static string GetDescription(IIndex startIndex, IIndex endIndex)
	{
		return (startIndex, endIndex) switch
		{
			(BeginningIndex, FileExtensionIndex) or
			(FixedIndex { Index: 0 }, FileExtensionIndex) => "file name",

			(FileExtensionIndex, EndIndex) => "file extension",

			(BeginningIndex, EndIndex) or
			(FixedIndex { Index: 0 }, EndIndex) => "all characters",

			(FixedIndex { Index: -1 }, EndIndex) => "the last character",
			(FixedIndex fix, EndIndex) when fix.Index < 0 => "the last " + "character".ToQuantity(-fix.Index),

			_ => $"characters from {startIndex.Description.ToString(includePreposition: false)} to {endIndex.Description.ToString(includePreposition: false)}",
		};
	}

	public static RangeValidationResult Validate(IIndex startIndex, IIndex endIndex)
	{
		return (startIndex, endIndex) switch
		{
			(_, BeginningIndex) or
			(EndIndex, _) or
			(FileExtensionIndex, FileExtensionIndex) => RangeValidationResult.InvalidIndexType,
			(FixedIndex i1, FixedIndex i2) when i2.Index <= i1.Index => RangeValidationResult.InvalidIndexPosition,
			_ => RangeValidationResult.Ok,
		};
	}
}

public enum RangeValidationResult
{
	Ok,
	InvalidIndexType,
	InvalidIndexPosition,
}