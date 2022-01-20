namespace FileRenamer.Core.FileSystem;

/// <summary>
/// Represents a folder in the target Operating System's file system.
/// Provides information about the folder and its content, and ways to manipulate them.
/// </summary>
public interface IFolder
{
	/// <summary>
	/// Gets the full path of the current folder in the file system.
	/// </summary>
	public string Path { get; }


	/// <summary>
	/// Gets the subfolders in the current folder.
	/// </summary>
	/// <remarks>This method performs a shallow query that returns only subfolders in the current folder.</remarks>
	/// <returns>When this method completes successfully, it returns a list of the subfolders in the current folder.</returns>
	public Task<IFolder[]> GetSubfoldersAsync();

	/// <summary>
	/// Gets the names of the files in the current folder.
	/// </summary>
	/// <remarks>This method performs a shallow query that returns only files in the current folder.</remarks>
	/// <returns>When this method completes successfully, it returns a list of the names of the files in the current folder.</returns>
	public Task<string[]> GetFileNamesAsync();

	/// <summary>
	/// Renames the file with the specified name in the current folder.
	/// </summary>
	/// <param name="name">The name of the file to be renamed.</param>
	/// <param name="newName">The desired, new name of the current item.</param>
	/// <returns>If this method completes successfully, <see langword="true"/>. Otherwise <see langword="false"/>.</returns>
	public Task<bool> RenameFileAsync(string name, string newName);
}