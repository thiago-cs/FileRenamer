using System.Xml;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core;

public sealed partial class Project : ObservableValidator
{
	public JobCollection Jobs { get; }

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
	/// <remarks>The default value is <see cref="JobScope.Files"/>.</remarks>
	[ObservableProperty]
	private JobScope _scope = JobScope.Files;


	public Project()
	{
		Jobs = new();
	}

	public Project(JobCollection jobs)
	{
		ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));

		Jobs = jobs;
	}


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


	#region XML serialization

	public async Task WriteXmlAsync(Stream output)
	{
		// 1. Locals.
		XmlWriterSettings settings = new() { Async = true };
		using XmlWriter writer = XmlWriter.Create(output, settings);

		// 2. The deal.
		await writer.WriteStartElementAsync(nameof(Project)).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(Scope), Scope.ToString()).ConfigureAwait(false);
		await writer.WriteElementAsync(nameof(Jobs), Jobs).ConfigureAwait(false);
		await writer.WriteEndElementAsync().ConfigureAwait(false);

		// 3. Writes the last closing element and flushes whatever is in the buffer.
		await writer.FlushAsync().ConfigureAwait(false);
	}

	public static async Task<Project> ReadXmlAsync(TextReader input)
	{
		XmlReaderSettings settings = new() { Async = true };
		using XmlReader reader = XmlReader.Create(input, settings);

		reader.MoveToContent();

		//
		JobScope scope = default;

		if (reader.AttributeCount != 0)
		{
			string? value = reader.GetAttribute(nameof(Scope));

			if (value != null)
				scope = Enum.Parse<JobScope>(value);
		}

		reader.ReadStartElement(nameof(Project));

		//
		JobCollection? jobs = null;

		while (reader.NodeType != XmlNodeType.EndElement)
			switch (reader.Name)
			{
				case nameof(Jobs):
					reader.ReadStartElement();
					jobs = await JobCollection.ReadXmlAsync(reader).ConfigureAwait(false);
					reader.ReadEndElement();
					break;

				default:
					break;
			}

		reader.ReadEndElement();

		//
		return new(jobs ?? new()) { Scope = scope };
	}

	#endregion
}