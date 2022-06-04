namespace FileRenamer.Core.FileSystem;

public interface IItem
{
	/// <summary>
	/// Gets the full path of this item.
	/// </summary>
	public string Path { get; }

	/// <summary>
	/// Gets the name of this item.
	/// </summary>
	public string Name => System.IO.Path.GetFileName(Path);

	/// <summary>
	/// Renames this item.
	/// </summary>
	/// <param name="newName">The desired, new name of this item.</param>
	/// <returns>If this method completes successfully, <see langword="true"/>. Otherwise <see langword="false"/>.</returns>
	Task<bool> RenameAsync(string newName);

	/// <summary>
	/// Moves this item into a new folder.
	/// </summary>
	/// <param name="newFolder">The new parent folder of this item.</param>
	/// <returns>If this method completes successfully, <see langword="true"/>. Otherwise <see langword="false"/>.</returns>
	public Task<bool> MoveAsync(IFolder newFolder);
}