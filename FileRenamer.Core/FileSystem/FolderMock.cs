namespace FileRenamer.Core.FileSystem;

/// <summary>
/// Mocks the <see cref="IFolder"/> from a path as a string.
/// </summary>
public sealed class FolderMock : IFolder
{
	private readonly ICollection<IFolder> subFolders;
	private readonly ICollection<IFile> subFiles;


	public string Path { get; private set; }


	public FolderMock(string path)
	{
		Path = path;
		subFolders = new List<IFolder>();
		subFiles = new List<IFile>();
	}

	public FolderMock(string path, ICollection<IFolder> subFolders, ICollection<IFile> subFiles)
	{
		Path = path;
		this.subFolders = subFolders;
		this.subFiles = subFiles;
	}


	public async Task<IFolder[]> GetSubfoldersAsync()
	{
		await Task.CompletedTask;

		return subFolders.ToArray();
	}

	public async Task<IFile[]> GetFilesAsync()
	{
		await Task.CompletedTask;

		return subFiles.ToArray();
	}

	public async Task<bool> RenameAsync(string newName)
	{
		await Task.CompletedTask;

		Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path) ?? "", newName);
		// todo: test whether this is allowed by a file table mock.
		return true;
	}

	public async Task<bool> MoveAsync(IFolder newFolder)
	{
		await Task.CompletedTask;

		string newPath = System.IO.Path.Combine(newFolder.Path, System.IO.Path.GetDirectoryName(Path)!);

		// todo: test whether this is allowed by a file table mock.

		IFolder[] subfolders = await newFolder.GetSubfoldersAsync();

		IFolder subfolder = subfolders.FirstOrDefault()
						 ?? await newFolder.CreateFolderAsync((this as IItem).Name);
		return true;
	}

	public async Task<IFolder> CreateFolderAsync(string name)
	{
		await Task.CompletedTask;

		string path = System.IO.Path.Combine(Path, name);
		FolderMock newFolder = new(path);
		subFolders.Add(newFolder);
		// todo: add the relationship between folders to a file table mock.
		return newFolder;
	}
}