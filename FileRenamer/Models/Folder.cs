using System;
using System.Linq;
using System.Threading.Tasks;
using FileRenamer.Core.FileSystem;
using Windows.Storage;


namespace FileRenamer.Models;

/// <summary>
/// Implements the <see cref="IFolder"/> interface for WinUI3 using a <see cref="StorageFolder"/>.
/// </summary>
internal class Folder : IFolder
{
	private StorageFolder folder;


	public string Path => folder.Path;


	#region Constructors

	public Folder(StorageFolder folder)
	{
		this.folder = folder;
	}

	public static implicit operator Folder(StorageFolder folder)
	{
		return new(folder);
	}

	public static implicit operator StorageFolder(Folder folder)
	{
		return folder;
	}

	#endregion


	public Task<IFolder[]> GetSubfoldersAsync()
	{
		throw new NotImplementedException();
	}

	public async Task<IFile[]> GetFilesAsync()
	{
		return (await folder.GetFilesAsync())
				.Select(file => new File(file))
				.ToArray();
	}

	public async Task<bool> RenameAsync(string newName)
	{
		try
		{
			await folder.RenameAsync(newName);
			return true;
		}
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
		catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
		{
			return false;
		}
	}

	public async Task<bool> MoveAsync(IFolder destination)
	{
		if (destination == null)
			return false;

		try
		{
			StorageFolder newParentFolder = destination is Folder f
										  ? f.folder
										  : await StorageFolder.GetFolderFromPathAsync(destination.Path);

			bool success = await folder.MoveAsync(newParentFolder);

			if (!success)
				return false;

			folder = await newParentFolder.GetFolderAsync(folder.Name);
		}
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
		catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
		{
			return false;
		}

		return true;
	}

	public async Task<IFolder> CreateFolderAsync(string name)
	{
		StorageFolder newFolder = await folder.CreateFolderAsync(name);
		return new Folder(newFolder);
	}
}