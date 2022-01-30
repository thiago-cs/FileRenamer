using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Core.Actions;
using FileRenamer.Core.FileSystem;


namespace FileRenamer.Core;

public sealed class Project : System.ComponentModel.BindableBase
{
	public ActionCollection Actions { get; } = new();

	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	public IFolder? Folder { get => _folder; set => SetProperty(ref _folder, value); }
	private IFolder? _folder;

	/// <summary>
	/// Gets a value between 0 and 100 representing the progress of the ongoing operation.
	/// </summary>
	public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }
	private double _progress;


	public async Task Run(CancellationToken cancellationToken, bool parallel = false)
	{
		// 0. 
		if (Folder == null)
		{
			//Error("Folder is null.");
			return;
		}

		// 1. 
		string[] fileNames = await Folder.GetFileNamesAsync();
		int counter = 0;

		if (parallel)
			await Parallel.ForEachAsync(fileNames, cancellationToken, RenameFile);
		else
			foreach (string fileName in fileNames)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				await Folder.RenameFileAsync(fileName, Actions.Run(fileName)).ConfigureAwait(true);
				counter++;
			}



		async ValueTask RenameFile(string fileName, CancellationToken _cancellationToken)
		{
			await Folder!.RenameFileAsync(fileName, Actions.Run(fileName)).ConfigureAwait(true);
			Interlocked.Add(ref counter, 1);
			Progress = counter;
		}
	}
}