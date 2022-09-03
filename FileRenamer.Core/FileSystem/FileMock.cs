namespace FileRenamer.Core.FileSystem;

/// <summary>
/// Mocks the <see cref="IFile"/> from a file path as a string.
/// </summary>
public sealed class FileMock : IFile
{
	private string fileName;
	private IFolder? folder;


	public string Path => folder == null ? fileName : System.IO.Path.Combine(folder.Path, fileName);


	public FileMock(string path)
	{
		fileName = System.IO.Path.GetFileName(path);

		string? folderPath = System.IO.Path.GetDirectoryName(path);

		folder = !string.IsNullOrEmpty(folderPath)
			   ? new FolderMock(folderPath)
			   : null;
	}


	public async Task<bool> RenameAsync(string newName)
	{
		await Task.CompletedTask;

		if (newName == null)
			return false;

		fileName = newName;
		return true;
	}

	public async Task<bool> MoveAsync(IFolder newFolder)
	{
		await Task.CompletedTask;

		if (newFolder == null)
			return false;

		folder = newFolder;
		return true;
	}
}