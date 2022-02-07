namespace FileRenamer.Core.Actions;

public abstract class RenameActionBase : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, Models.ICloneable<RenameActionBase>
{
	private bool _isEnabled = true;
	public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

	private string _description = "";
	public string Description { get => _description; set => SetProperty(ref _description, value); }


	public abstract string Run(string input);

	public abstract void UpdateDescription();

	/// <summary>
	/// Creates a new object that is a shallow copy of the current instance.
	/// </summary>
	/// <returns>A new RenameActionBase that is a copy of this instance.</returns>
	public abstract RenameActionBase Clone();


#if DEBUG
	protected string GetDebuggerDisplay()
	{
		return Description;
	}
#endif
}