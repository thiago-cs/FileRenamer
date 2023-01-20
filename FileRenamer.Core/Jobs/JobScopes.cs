using System.ComponentModel;


namespace FileRenamer.Core.Jobs;

/// <summary>
/// Represents the job scope of a <see cref="Project"/>, indicating whether files, folders, or both should be manipulated.
/// </summary>
[Flags]
public enum JobScopes
{
	/// <summary>
	/// Indicates that neither files or folders should be manipulated.
	/// </summary>
	[Description("None")]
	None,

	/// <summary>
	/// Indicates that only files should be manipulated.
	/// </summary>
	[Description("Files")]
	Files = 1 << 0,

	/// <summary>
	/// Indicates that only folders should be manipulated.
	/// </summary>
	[Description("Folders")]
	Folders = 1 << 1,

	/// <summary>
	/// Indicates that files and folders should be manipulated.
	/// </summary>
	[Description("Files and folders")]
	FilesAndFolders = Files | Folders,
}