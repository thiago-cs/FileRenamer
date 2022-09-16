using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Models;
using FileRenamer.ViewModels;


namespace FileRenamer;

public sealed partial class MainWindow
{
	internal MainWindowViewModel ViewModel { get; }


	public MainWindow()
	{
		ViewModel = new(this);

		InitializeComponent();
		//ExtendsContentIntoTitleBar = true;
	}


	// Cannot be moved to a view-model because the dialog shown requires a window reference.
	private async void PickFolderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		FolderPicker picker = new()
		{
			SuggestedStartLocation = PickerLocationId.Downloads,
			ViewMode = PickerViewMode.List,
			FileTypeFilter = { "*", },
		};

		// Make folder Picker work in Win32
		picker.SetOwnerWindow(this);

		// Use file picker like normal!
		Windows.Storage.StorageFolder folder = await picker.PickSingleFolderAsync();

		if (folder == null)
			return;

		ViewModel.Project.Folder = new Folder(folder);
		ViewModel.DoItCommand.NotifyCanExecuteChanged();
	}

	public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
	{
		Panel panel = Content as Panel;
		panel.Children.Add(dialog);

		ContentDialogResult result = await dialog.ShowAsync();

		panel.Children.Remove(dialog);
		return result;
	}

	public async Task<ContentDialogResult> ShowJobEditorDialogAsync(UserControls.ActionEditors.IActionEditor editor)
	{
		dialog.DataContext = editor;

		ContentDialogResult result = await dialog.ShowAsync();

		return result;
	}
}