using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public enum ExecutionScope
{
	[Description("everything")]
	WholeInput,

	[Description("range")]
	Range,

	[Description("all occurrencies of")]
	Occurrences,
}