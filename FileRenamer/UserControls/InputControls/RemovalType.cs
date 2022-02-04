using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public enum RemovalType
{
	[Description("count")]
	FixedLength,

	[Description("until")]
	EndIndex,
}