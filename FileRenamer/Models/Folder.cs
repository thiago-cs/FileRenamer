using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Core.FileSystem;


namespace FileRenamer.Models;

/// <summary>
/// Implements the <see cref="IFolder"/> interface for WinUI3 using a <see cref="Windows.Storage.StorageFolder"/>.
/// </summary>
internal class Folder : IFolder
{
	private readonly Windows.Storage.StorageFolder folder;


	/// <inheritdoc/>
	public string Path => folder.Path;


	#region Constructors

	public Folder(Windows.Storage.StorageFolder folder)
	{
		this.folder = folder;
	}

	public static implicit operator Folder(Windows.Storage.StorageFolder folder)
	{
		return new(folder);
	}

	public static implicit operator Windows.Storage.StorageFolder(Folder folder)
	{
		return folder;
	}

	#endregion


	/// <inheritdoc/>
	public Task<IFolder[]> GetSubfoldersAsync()
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public Task<IFile[]> GetFilesAsync()
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	public async Task<bool> MoveAsync(IFolder newFolder)
	{
		try
		{
			IFolder[] subfolders = await newFolder.GetSubfoldersAsync();

			IFolder subfolder = subfolders.FirstOrDefault()
							 ?? await newFolder.CreateFolderAsync((this as IItem).Name);
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

	/// <inheritdoc/>
	public async Task<IFolder> CreateFolderAsync(string name)
	{
		Windows.Storage.StorageFolder newFolder = await folder.CreateFolderAsync(name);
		return new Folder(newFolder);
	}
}