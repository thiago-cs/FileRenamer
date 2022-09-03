using FileRenamer.Core.Jobs;

namespace FileRenamer.Core.ValueSources;

public sealed class FolderNameValueSource : IValueSource
{
	public string Description => $"the folder name";


	public string GetValue(JobTarget target)
	{
		return Path.GetFileName(target.FolderName);
	}
}