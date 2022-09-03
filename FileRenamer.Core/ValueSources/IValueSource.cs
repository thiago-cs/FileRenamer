using FileRenamer.Core.Jobs;


namespace FileRenamer.Core.ValueSources;

public interface IValueSource
{
	/// <summary>
	/// Gets the description for the value that is provided by this <see cref="IValueSource"/> instance.
	/// </summary>
	string Description { get; }

	string GetValue(JobTarget target);
}