using FileRenamer.Core.FileSystem;


namespace FileRenamer.Core.Jobs;

// Transient data. Non-serializable.
public sealed class JobTarget
{
	/// <summary>
	/// Gets the path of the file being processed.
	/// </summary>
	private readonly string filePath;


	/// <summary>
	/// Gets the name of the file being processed.
	/// </summary>
	public string FileName => Path.GetFileName(filePath);

	/// <summary>
	/// Gets the name of the folder where the file being processed is located.
	/// </summary>
	public string FolderName => Path.GetDirectoryName(filePath) ?? string.Empty;

	/// <summary>
	/// Gets or sets the new name for the file being processed.
	/// </summary>
	public string NewFileName {get; set; }

	/// <summary>
	/// Gets or sets the new path for the folder where the file being processed is located.
	/// </summary>
	public string NewFolderName {get; set; }

	/// <summary>
	/// Gets the new path of the file being processed.
	/// </summary>
	public string NewPath => Path.Combine(NewFolderName, NewFileName);

	public IItem StorageItem { get; }

	/// <summary>
	/// Gets the index of the current item.
	/// </summary>
	public int Index { get; }

	///// <summary>
	///// Gets the number of target items in this execution context.
	///// </summary>
	//public int Count => Context.Targets.Count;

	///// <summary>
	///// Gets or sets the occurrences of a specified string or regular expression.
	///// </summary>
	//public string[]? Matches { get; set; }


	/// <summary>
	/// Initializes a new instance of this class.
	/// </summary>
	public JobTarget(IItem storageItem, int index)
	{
		StorageItem = storageItem;
		filePath = storageItem.Path;
		NewFileName = FileName;
		NewFolderName = FolderName;
		Index = index;
	}
}