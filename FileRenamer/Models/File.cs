using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Core.FileSystem;


namespace FileRenamer.Models;

/// <summary>
/// Implements the <see cref="IFile"/> interface for WinUI3 using a <see cref="Windows.Storage.StorageFile"/>.
/// </summary>
internal class File : IFile
{
	private readonly Windows.Storage.StorageFile file;


	/// <inheritdoc/>
	public string Path => file.Path;


	#region Constructors

	public File(Windows.Storage.StorageFile file)
	{
		this.file = file;
	}

	public static implicit operator File(Windows.Storage.StorageFile file)
	{
		return new(file);
	}

	#endregion


	/// <inheritdoc/>
	public async Task<bool> RenameAsync(string newName)
	{
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
		try
		{
			await file.RenameAsync(newName);
			return true;
		}
		catch (Exception ex)
		{
			return false;
		}
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
	}

	/// <inheritdoc/>
	public async Task<bool> MoveAsync(IFolder newFolder)
	{
		try
		{
			await file.MoveAsync((Windows.Storage.StorageFolder)(newFolder as Folder));
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
}