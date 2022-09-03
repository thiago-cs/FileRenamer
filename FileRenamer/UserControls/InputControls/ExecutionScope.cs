using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

/// <summary>
/// Represents the execution scope of an operation over a string.
/// </summary>
public enum ExecutionScope
{
	/// <summary>
	/// The operation should be executed only over the file name.
	/// </summary>
	[Description("file name")]
	FileName,

	/// <summary>
	/// The operation should be executed only over the file extension.
	/// </summary>
	[Description("file extension")]
	FileExtension,

	/// <summary>
	/// The operation should be executed over the whole string.
	/// </summary>
	[Description("everything")]
	WholeInput,

	/// <summary>
	/// The operation should be executed over a defined range of the string.
	/// </summary>
	[Description("custom range")]
	CustomRange,

	/// <summary>
	/// The operation should be executed over all the ranges that satisfy a given condition.
	/// </summary>
	[Description("all occurrencies of")]
	Occurrences,
}