using System.ComponentModel;


namespace FileRenamer.Core.Jobs;

/// <summary>
/// Represents the job scope of a <see cref="Project"/>, indicating whether files, folders, or both should be manipulated.
/// </summary>
public enum JobScope
{
	/// <summary>
	/// Indicates that only files should be manipulated.
	/// </summary>
	[Description("Files")]
	Files,

	/// <summary>
	/// Indicates that only folders should be manipulated.
	/// </summary>
	[Description("Folders")]
	Folders,

	/// <summary>
	/// Indicates that files and folders should be manipulated.
	/// </summary>
	[Description("Files and folders")]
	FilesAndFolders,
}