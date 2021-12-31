namespace FileRenamer.Core.Actions;


public abstract class RenameActionBase : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
{
	private bool _isEnabled = true;
	public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }


	public abstract string Run(string input);
}