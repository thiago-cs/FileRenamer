namespace FileRenamer.Core.Jobs.FileActions;

/// <summary>
/// Base class for all action items that modify the name of the <see cref="JobTarget"/> when it is run.
/// </summary>
public abstract partial class RenameFileJob : FileActionJob
{
	// todo: make this protected.
	public abstract void UpdateDescription();
}