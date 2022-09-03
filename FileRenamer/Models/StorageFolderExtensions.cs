using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace FileRenamer.Models;

// todo: test these StorageFolder.MoveAsync extension methods.
public static class StorageFolderExtensions
{
	public static async Task<bool> MoveAsync(this StorageFolder folder, string destination)
	{
		// . 
		StorageFolder newParent;
		try
		{
			newParent = await StorageFolder.GetFolderFromPathAsync(destination);
		}
		catch (Exception ex)
		{
			throw new Exception("", ex);
		}

		return await folder.MoveAsync(newParent);
	}

	public static async Task<bool> MoveAsync(this StorageFolder folder, StorageFolder destination)
	{
		// 1. 
		StorageFolder newfolder = null;
		try
		{
			newfolder = await destination.GetFolderAsync(folder.Name);
		}
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
		catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
		{ }

		bool createdNewFolder = false;
		if (newfolder == null)
			try
			{
				folder = await folder.CreateFolderAsync(folder.Name);
				createdNewFolder = true;
			}
			catch (Exception ex)
			{
				throw new Exception("", ex);
			}

		// 2. 
		IReadOnlyList<StorageFile> files;
		try
		{
			files = await folder.GetFilesAsync();
		}
		catch (Exception ex)
		{
			if (createdNewFolder)
				await newfolder.DeleteAsync();

			throw new Exception("", ex);
		}

		try
		{
			foreach (StorageFile file in files)
				await file.MoveAsync(folder);
		}
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
		catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
		{
			return false;
		}

		// 3. 
		IReadOnlyList<StorageFolder> subfolders;
		try
		{
			subfolders = await folder.GetFoldersAsync();

			foreach (StorageFolder subfolder in subfolders)
				await folder.MoveAsync(subfolder);
		}
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
		catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
		{
			return false;
		}

		// 4. 
		await folder.DeleteAsync();
		return true;
	}
}