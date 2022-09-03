using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public enum TextRangeType
{
	[Description("count")]
	Count,

	[Description("until")]
	Range,
}