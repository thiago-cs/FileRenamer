using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Models;
using FileRenamer.ViewModels;
using FileRenamer.UserControls.ActionEditors;

namespace FileRenamer;

public sealed partial class MainWindow
{
	internal MainWindowViewModel ViewModel { get; } = new();


	public MainWindow()
	{
		// 1. 
		InitializeComponent();
		//ExtendsContentIntoTitleBar = true;

		#region 2. Commands

		const VirtualKeyModifiers Control_Shift = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift;

		// 2.1. Project commands
		NewProjectCommand = CreateCommand2("New", "N", "Start a new project",
											Symbol.Add, VirtualKeyModifiers.Control, VirtualKey.N, ExecuteNewProject);

		LoadProjectCommand = CreateCommand2("Load", "L", "Load an existing project",
											Symbol.OpenLocal, VirtualKeyModifiers.Control, VirtualKey.O, ExecuteLoadProject);

		SaveProjectCommand = CreateCommand2("Save", "S", "Save this project",
											Symbol.Save, VirtualKeyModifiers.Control, VirtualKey.S, ExecuteSaveProject);

		// 2.2. Manage existing actions commands
		MoveUpActionCommand = CreateCommand2("Move up", "U", "Move the selected action up",
											Symbol.Up, VirtualKeyModifiers.Menu, VirtualKey.Up, ViewModel.MoveSelectedActionUp, ViewModel.CanExecuteWhenSelectedActionIsNotFirst);

		MoveDownActionCommand = CreateCommand3("Move down", "D", "Move the selected action down",
											(char)0xE74B, VirtualKeyModifiers.Menu, VirtualKey.Down, ViewModel.MoveSelectedActionDown, ViewModel.CanExecuteWhenSelectedActionIsNotLast);

		EditActionCommand = CreateCommand2("Edit", "T", "Edit the selected action",
											Symbol.Edit, null, VirtualKey.F2, EditSelectedAction, ViewModel.CanExecuteWhenSelectedActionIsNotNull);

		DuplicateActionCommand = CreateCommand2("Duplicate", "V", "Duplicate the selected action",
											Symbol.Copy, VirtualKeyModifiers.Control, VirtualKey.D, ViewModel.DuplicateSelectedAction, ViewModel.CanExecuteWhenSelectedActionIsNotNull);

		RemoveActionCommand = CreateCommand2("Remove", "Del", "Remove the selected action",
											Symbol.Delete, null, VirtualKey.Delete, ViewModel.RemoveSelectedAction, ViewModel.CanExecuteWhenSelectedActionIsNotNull);

		RemoveAllActionCommand = CreateCommand2("Clear", "", "Remove all actions",
											Symbol.Clear, VirtualKeyModifiers.Control, VirtualKey.Delete, ViewModel.RemoveAllActions, ViewModel.CanExecuteWhenActionsIsNotEmpty);

		// 2.3. Add new actions commands
		AddInsertActionCommand = CreateCommand2("Insert", "I", "Add an action that inserts a text",
												Symbol.Add, VirtualKeyModifiers.Control, VirtualKey.I, AddInsertAction);

		AddRemoveActionCommand = CreateCommand2("Remove", "R", "Add an action that removes text",
												Symbol.Remove, VirtualKeyModifiers.Control, VirtualKey.R, AddRemoveAction);

		AddInsertCounterActionCommand = CreateCommand3("Insert counter", "1", "Add an action that inserts a padded number",
												(char)0xE8EF, Control_Shift, VirtualKey.I, AddInsertCounterAction);

		AddReplaceActionCommand = CreateCommand2("Replace", "H", "Add an action that replaces a text",
												Symbol.Sync, Control_Shift, VirtualKey.R, AddReplaceAction);

		AddConvertCaseActionCommand = CreateCommand2("Change case", "C", "Add an action that changes the casing of a text",
												Symbol.Font, Control_Shift, VirtualKey.C, AddConvertCaseAction);

		AddMoveStringActionCommand = CreateCommand3("Move Text", "M", "Move a text around",
												(char)0xE8AB, Control_Shift, VirtualKey.M, AddConvertCaseAction);

		#endregion

		// 3.
		ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		ViewModel.Project.Jobs.CollectionChanged += Actions_CollectionChanged;



		#region Local functions

		static UICommand CreateCommand(string label, string accessKey, string description, IconSource icon,
									   VirtualKeyModifiers? modifier, VirtualKey acceleratorKey, Action execute, Func<bool> canExecute)
		{
			return new UICommand(execute, canExecute)
			{
				Label = label,
				Description = description,
				IconSource = icon,
				AccessKey = accessKey,
				KeyboardAccelerator = CreateKeyboardAccelerator(modifier, acceleratorKey)
			};
		}

		static UICommand CreateCommand2(string label, string accessKey, string description, Symbol symbol,
										VirtualKeyModifiers? modifier, VirtualKey acceleratorKey, Action execute, Func<bool> canExecute = null)
		{
			return CreateCommand(label, accessKey, description, new SymbolIconSource() { Symbol = symbol }, modifier, acceleratorKey, execute, canExecute);
		}

		static UICommand CreateCommand3(string label, string accessKey, string description, char glyph,
										VirtualKeyModifiers? modifier, VirtualKey acceleratorKey, Action execute, Func<bool> canExecute = null)
		{
			return CreateCommand(label, accessKey, description, new FontIconSource() { Glyph = glyph.ToString() }, modifier, acceleratorKey, execute, canExecute);
		}

		static KeyboardAccelerator CreateKeyboardAccelerator(VirtualKeyModifiers? modifier, VirtualKey acceleratorKey)
		{
			KeyboardAccelerator keyboardAccelerator = new() { Key = acceleratorKey };

			if (modifier != null)
				keyboardAccelerator.Modifiers = modifier.Value;

			return keyboardAccelerator;
		}

		#endregion
	}


	private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(MainWindowViewModel.SelectedAction):
				UpdateCommandStates();
				break;
		}
	}

	private void Actions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		UpdateCommandStates();
	}


	#region Commands

	#region Project Commands

	public readonly UICommand NewProjectCommand;
	public readonly UICommand LoadProjectCommand;
	public readonly UICommand SaveProjectCommand;


	private void ExecuteNewProject()
	{
	}

	private void ExecuteLoadProject()
	{
	}

	private void ExecuteSaveProject()
	{
	}

	#endregion

	#region Manage existing actions commands

	public readonly UICommand MoveUpActionCommand;
	public readonly UICommand MoveDownActionCommand;
	public readonly UICommand EditActionCommand;
	public readonly UICommand DuplicateActionCommand;
	public readonly UICommand RemoveActionCommand;
	public readonly UICommand RemoveAllActionCommand;


	private async void EditSelectedAction()
	{
		//
		if (ViewModel.SelectedAction == null)
		{
			// Oops!
			return;
		}

		//
		IActionEditor actionEditor = ViewModel.SelectedAction switch
		{
			InsertAction action => new InsertActionEditor(action),
			RemoveAction action => new RemoveActionEditor(action),
			ReplaceAction action => new ReplaceActionEditor(action),
			ChangeRangeCaseAction action => new ChangeCaseActionEditor(action),
			ChangeStringCaseAction action => new ChangeCaseActionEditor(action),
			_ => null,
		};

		//
		IJobItem item = await EditJobItemInDialog(actionEditor);

		if (item == null)
			return;

		int index = ViewModel.Project.Jobs.IndexOf(ViewModel.SelectedAction);
		ViewModel.Project.Jobs.RemoveAt(index);
		ViewModel.Project.Jobs.Insert(index, item);
	}

	#endregion

	#region Add new actions commands

	public readonly UICommand AddInsertActionCommand;
	public readonly UICommand AddInsertCounterActionCommand;
	public readonly UICommand AddRemoveActionCommand;
	public readonly UICommand AddReplaceActionCommand;
	public readonly UICommand AddConvertCaseActionCommand;
	public readonly UICommand AddMoveStringActionCommand;


	private async void AddInsertAction()
	{
		await EditAndAddJobItem(new InsertActionEditor());
	}

	private async void AddInsertCounterAction()
	{
		InsertActionEditor actionEditor = new();
		actionEditor.Data.ValueSourceType = UserControls.InputControls.ValueSourceType.Counter;

		await EditAndAddJobItem(actionEditor);
	}

	private async void AddRemoveAction()
	{
		await EditAndAddJobItem(new RemoveActionEditor());
	}

	private async void AddReplaceAction()
	{
		await EditAndAddJobItem(new ReplaceActionEditor());
	}

	private async void AddConvertCaseAction()
	{
		await EditAndAddJobItem(new ChangeCaseActionEditor());
	}


	private async Task<IJobItem> EditJobItemInDialog(IActionEditor actionEditor)
	{
		if (actionEditor == null)
		{
			// Oops!
			return null;
		}

		//
		dialog.DataContext = actionEditor;

		ContentDialogResult result = await dialog.ShowAsync();

		if (result != ContentDialogResult.Primary)
			return null;

		if (!actionEditor.IsValid)
		{
			// Oops!
			return null;
		}

		return actionEditor.GetRenameAction();
	}

	private async Task EditAndAddJobItem(IActionEditor actionEditor)
	{
		IJobItem newAction = await EditJobItemInDialog(actionEditor);

		if (newAction == null)
			return;

		ViewModel.Project.Jobs.Add(newAction);
		ViewModel.SelectedAction = newAction;
	}

	#endregion

	private void UpdateCommandStates()
	{
		MoveUpActionCommand.NotifyCanExecuteChanged();
		MoveDownActionCommand.NotifyCanExecuteChanged();
		EditActionCommand.NotifyCanExecuteChanged();
		DuplicateActionCommand.NotifyCanExecuteChanged();
		RemoveActionCommand.NotifyCanExecuteChanged();
		RemoveAllActionCommand.NotifyCanExecuteChanged();
	}

	#endregion


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
	}
}