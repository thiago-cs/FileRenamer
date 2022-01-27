using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public enum TextType
{
	[Description("text")]
	Text,

	[Description("regex")]
	Regex,
}