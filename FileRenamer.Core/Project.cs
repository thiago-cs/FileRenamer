using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;


namespace FileRenamer.Core;

public sealed partial class Project : ObservableValidator
{
	public JobCollection Jobs { get; } = new();

	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	[ObservableProperty]
	private IFolder? _folder;

	private double _progress;
	/// <summary>
	/// Gets a value between 0 and 100 representing the progress of the ongoing operation.
	/// </summary>
	public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }

	/// <summary>
	/// Gets or sets a value that indicates whether files, folders, or both should be manipulated.
	/// </summary>
	[ObservableProperty]
	private JobScope _scope;


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