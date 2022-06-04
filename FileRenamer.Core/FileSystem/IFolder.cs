namespace FileRenamer.Core.FileSystem;

/// <summary>
/// Represents a folder in the target Operating System's file system.
/// Provides information about the folder and its content, and ways to manipulate them.
/// </summary>
public interface IFolder : IItem
{
	/// <summary>
	/// Gets the subfolders in the current folder.
	/// </summary>
	/// <remarks>This method performs a shallow query that returns only subfolders in the current folder.</remarks>
	/// <returns>When this method completes, it returns a list of the subfolders in the current folder.</returns>
	public Task<IFolder[]> GetSubfoldersAsync();

	/// <summary>
	/// Gets the names of the files in the current folder.
	/// </summary>
	/// <remarks>This method performs a shallow query that returns only files in the current folder.</remarks>
	/// <returns>When this method completes, it returns a list of the names of the files in the current folder.</returns>
	public Task<IFile[]> GetFilesAsync();

	/// <summary>
	/// Creates a new subfolder with the specified name in the current folder.
	/// </summary>
	/// <param name="name">The name of the new subfolder to create in the current folder.</param>
	/// <returns>When this method completes, it returns an <see cref="IFolder"/> that represents the new subfolder.</returns>
	Task<IFolder> CreateFolderAsync(string name);
}