using FileRenamer.Core;


namespace FileRenamer.ViewModels;

public sealed class MainWindowViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
{
	private Project _project;
	public Project Project { get => _project; set => SetProperty(ref _project, value); }
}