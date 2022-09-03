using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;


namespace FileRenamer.Core;

public sealed class Project : System.ComponentModel.BindableBase
{
	public JobCollection Jobs { get; } = new();

	private IFolder? _folder;
	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	public IFolder? Folder { get => _folder; set => SetProperty(ref _folder, value); }

	private double _progress;
	/// <summary>
	/// Gets a value between 0 and 100 representing the progress of the ongoing operation.
	/// </summary>
	public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }

	private JobScope _scope;
	/// <summary>
	/// Gets or sets a value that indicates whether files, folders, or both should be manipulated.
	/// </summary>
	public JobScope Scope { get => _scope; set => SetProperty(ref _scope, value); }


	public async Task RunAsync(CancellationToken cancellationToken)
	{
		// 0. 
		if (Folder == null)
		{
			//Error("Folder is null.");
			return;
		}

		// 1. 
		// 1.1. 
		IFile[] files = await Folder.GetFilesAsync();
		JobTarget[] targets = files.Select((file, i) => new JobTarget(file, i)).ToArray();
		JobContext context = new(Jobs, targets);

		// 1.2. 
		for (int i = 0; i < files.Length; i++)
		{
			//
			if (cancellationToken.IsCancellationRequested)
				break;

			//
			IFile file = files[i];
			JobTarget target = targets[i];

			//
			Jobs.Run(target, context);
			await file.RenameAsync(target.NewFileName).ConfigureAwait(true);

			//
			Progress = i;
		}
	}
}