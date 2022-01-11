namespace FileRenamer.Core.Actions;

public abstract class RenameActionBase : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
{
	private bool _isEnabled = true;
	public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

	private string _description = "";
	public string Description { get => _description; set => SetProperty(ref _description, value); }


	public abstract string Run(string input);
}