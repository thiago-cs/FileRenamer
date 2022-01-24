using FileRenamer.Core;
using FileRenamer.Core.Actions;


namespace FileRenamer.ViewModels;

public sealed class MainWindowViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
{
	private Project _project = new();
	public Project Project { get => _project; set => SetProperty(ref _project, value); }

	private RenameActionBase _selectedAction;
	public RenameActionBase SelectedAction { get => _selectedAction; set => SetProperty(ref _selectedAction, value); }
}