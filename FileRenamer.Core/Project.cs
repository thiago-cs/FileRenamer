using System.Xml;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core;

public sealed partial class Project : ObservableValidator
{
	#region Jobs

	public JobCollection Jobs { get; }

	private void Jobs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		HasUnsavedChanges = true;
	}

	private void Jobs_NestedJobChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		HasUnsavedChanges= true;
	}

	#endregion

	#region Progress

	private double _progress;
	/// <summary>
	/// Gets a value between 0 and 100 representing the progress of the ongoing operation.
	/// </summary>
	public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }

	#endregion

	#region Scope

	/// <summary>
	/// Gets or sets a value that indicates whether files, folders, or both should be manipulated.
	/// </summary>
	/// <remarks>The default value is <see cref="JobScopes.Files"/>.</remarks>
	[ObservableProperty]
	private JobScopes _scope = JobScopes.Files;

	partial void OnScopeChanged(JobScopes value)
	{
		HasUnsavedChanges = true;
	}

	#endregion

	#region HasUnsavedChanges

	[ObservableProperty]
	private bool _hasUnsavedChanges = false;

	#endregion


	#region Constructors

	public Project()
	{
		Jobs = new();
		Jobs.CollectionChanged += Jobs_CollectionChanged;
		Jobs.NestedJobChanged += Jobs_NestedJobChanged;
	}

	public Project(JobCollection jobs)
	{
		ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));

		Jobs = jobs;
		Jobs.CollectionChanged += Jobs_CollectionChanged;
		Jobs.NestedJobChanged += Jobs_NestedJobChanged;
	}

	~Project()
	{
		Jobs.CollectionChanged -= Jobs_CollectionChanged;
		Jobs.NestedJobChanged -= Jobs_NestedJobChanged;
	}

	#endregion


	public JobTarget[] ComputeChanges(IList<IItem> items)
	{
		JobTarget[] targets = items.Select((item, i) => new JobTarget(item, i)).ToArray();
		JobContext context = new(Jobs, targets);

		for (int i = 0; i < targets.Length; i++)
		{
			bool shouldRun = Scope switch
			{
				JobScopes.None => false,
				JobScopes.Files => items[i] is IFile,
				JobScopes.Folders => items[i] is IFolder,
				JobScopes.FilesAndFolders => true,
				_ => throw new NotImplementedException(@$"Unknown {nameof(JobScopes)} ""{Scope}""."),
			};

			if (shouldRun)
				Jobs.Run(targets[i], context);
		}

		return targets;
	}

	public async Task RunAsync(IFolder? folder, CancellationToken cancellationToken)
	{
		// 0. 
		if (folder == null)
		{
			//Error("Folder is null.");
			return;
		}

		// 1. 
		IList<IItem> items = await GetItems(folder, Scope);

		JobTarget[] targets = ComputeChanges(items);

		for (int i = 0; i < items.Count; i++)
		{
			if (cancellationToken.IsCancellationRequested)
				break;

			await targets[i].StorageItem.RenameAsync(targets[i].NewFileName).ConfigureAwait(true);
			Progress = i;
		}
	}

	private static async Task<IList<IItem>> GetItems(IFolder folder, JobScopes scope)
	{
		switch (scope)
		{
			case JobScopes.None:
				return new List<IItem>();

			case JobScopes.Files:
				return await folder.GetFilesAsync();

			case JobScopes.Folders:
				return await folder.GetSubfoldersAsync();

			case JobScopes.FilesAndFolders:
				List<IItem> list = new();
				list.AddRange(await folder.GetSubfoldersAsync());
				list.AddRange(await folder.GetFilesAsync());
				return list;

			default:
				throw new Exception(@$"Unknown {nameof(JobScopes)} value ""{scope}"".");
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
		output.SetLength(output.Position);
		await writer.FlushAsync().ConfigureAwait(false);

		// 4.
		HasUnsavedChanges = false;
	}

	public static async Task<Project> ReadXmlAsync(TextReader input)
	{
		// 0.
		if (input == null)
			throw new ArgumentNullException(nameof(input));

		// 1.
		XmlReaderSettings settings = new() { Async = true, IgnoreWhitespace = true, };
		using XmlReader reader = XmlReader.Create(input, settings);

		reader.MoveToContent();

		// 2.
		JobScopes scope = default;

		if (reader.AttributeCount != 0)
		{
			string? value = reader.GetAttribute(nameof(Scope));

			if (value != null)
				scope = Enum.Parse<JobScopes>(value);
		}

		reader.ReadStartElement(nameof(Project));

		// 3.
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
					reader.Skip();
					break;
			}

		reader.ReadEndElement();

		//
		return new(jobs ?? new()) { Scope = scope };
	}

	#endregion
}