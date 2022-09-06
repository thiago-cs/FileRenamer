using System;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Models;
using FileRenamer.UserControls.ActionEditors;


namespace FileRenamer;

partial class MainWindow
{
	#region Project Commands

	#region New Project

	private UICommand _newProjectCommand;
	public UICommand NewProjectCommand => _newProjectCommand ??= new(
		description: "Start a new project",
		label: "New",
		accessKey: "N",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.N,
		icon: CreateIconFromSymbol(Symbol.Add),
		execute: NewProject);

	private void NewProject()
	{
	}

	#endregion

	#region Load Project

	private UICommand _loadProjectCommand;
	public UICommand LoadProjectCommand => _loadProjectCommand ??= new(
		description: "Load an existing project",
		label: "Load",
		accessKey: "L",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.O,
		icon: CreateIconFromSymbol(Symbol.OpenLocal),
		execute: LoadProject);

	private void LoadProject()
	{
	}

	#endregion

	#region Save Project

	private UICommand _saveProjectCommand;
	public UICommand SaveProjectCommand => _saveProjectCommand ??= new(
		description: "Save this project",
		label: "Save",
		accessKey: "S",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.S,
		icon: CreateIconFromSymbol(Symbol.Save),
		execute: SaveProject,
		canExecute: CanSaveProject);

	private void SaveProject()
	{
	}

	private bool CanSaveProject()
	{
		return true;
	}

	#endregion

	#endregion

	#region Manage existing actions commands

	#region Move Up

	private UICommand _moveUpActionCommand;
	public UICommand MoveUpActionCommand => _moveUpActionCommand ??= new(
		description: "Move the selected action up",
		label: "Move up",
		accessKey: "U",
		modifier: VirtualKeyModifiers.Menu,
		acceleratorKey: VirtualKey.Up,
		icon: CreateIconFromSymbol(Symbol.Up),
		execute: ViewModel.MoveSelectedActionUp,
		canExecute: ViewModel.CanExecuteWhenSelectedActionIsNotFirst);

	#endregion

	#region Move Down

	private UICommand _moveDownActionCommand;
	public UICommand MoveDownActionCommand => _moveDownActionCommand ??= new(
		description: "Move the selected action down",
		label: "Move down",
		accessKey: "D",
		modifier: VirtualKeyModifiers.Menu,
		acceleratorKey: VirtualKey.Down,
		icon: CreateIconFromGlyph((char)0xE74B),
		execute: ViewModel.MoveSelectedActionDown,
		canExecute: ViewModel.CanExecuteWhenSelectedActionIsNotLast);

	#endregion

	#region Edit

	private AsyncUICommand _editActionCommand;
	public AsyncUICommand EditActionCommand => _editActionCommand ??= new(
		description: "Edit the selected action",
		label: "Edit",
		accessKey: "T",
		modifier: null,
		acceleratorKey: VirtualKey.F2,
		icon: CreateIconFromSymbol(Symbol.Edit),
		execute: EditSelectedActionAsync,
		canExecute: ViewModel.CanExecuteWhenSelectedActionIsNotNull);

	private async Task EditSelectedActionAsync()
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
			MoveStringAction action => new MoveStringActionEditor(action),
			_ => null,
		};

		//
		IJobItem item = await EditJobItemInDialogAsync(actionEditor);

		if (item == null)
			return;

		int index = ViewModel.Project.Jobs.IndexOf(ViewModel.SelectedAction);
		ViewModel.Project.Jobs.RemoveAt(index);
		ViewModel.Project.Jobs.Insert(index, item);
	}

	#endregion

	#region Duplicate

	private UICommand _duplicateActionCommand;
	public UICommand DuplicateActionCommand => _duplicateActionCommand ??= new(
		description: "Duplicate the selected action",
		label: "Duplicate",
		accessKey: "V",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.D,
		icon: CreateIconFromSymbol(Symbol.Copy),
		execute: ViewModel.DuplicateSelectedAction,
		canExecute: ViewModel.CanExecuteWhenSelectedActionIsNotNull);

	#endregion

	#region Remove selected

	private UICommand _removeActionCommand;
	public UICommand RemoveActionCommand => _removeActionCommand ??= new(
		description: "Remove the selected action",
		label: "Remove",
		accessKey: "Del",
		modifier: null,
		acceleratorKey: VirtualKey.Delete,
		icon: CreateIconFromSymbol(Symbol.Delete),
		execute: ViewModel.RemoveSelectedAction,
		canExecute: ViewModel.CanExecuteWhenSelectedActionIsNotNull);

	#endregion

	#region Remove All

	private UICommand _removeAllActionsCommand;
	public UICommand RemoveAllActionsCommand => _removeAllActionsCommand ??= new(
		description: "Remove all actions",
		label: "Clear",
		accessKey: "",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.Delete,
		icon: CreateIconFromSymbol(Symbol.Clear),
		execute: ViewModel.RemoveAllActions,
		canExecute: ViewModel.CanExecuteWhenActionsIsNotEmpty);

	#endregion

	#endregion

	#region Add new actions commands

	#region Add InsertAction

	private AsyncUICommand _addInsertActionCommand;
	public AsyncUICommand AddInsertActionCommand => _addInsertActionCommand ??= new(
		description: "Add an action that inserts a text",
		label: "Insert",
		accessKey: "I",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.I,
		icon: CreateIconFromSymbol(Symbol.Add),
		execute: AddInsertActionAsync);

	private async Task AddInsertActionAsync()
	{
		await EditAndAddJobItemAsync(new InsertActionEditor());
	}

	#endregion

	#region Add InsertCounterAction

	private AsyncUICommand _addInsertCounterActionCommand;
	public AsyncUICommand AddInsertCounterActionCommand => _addInsertCounterActionCommand ??= new(
		description: "Add an action that inserts a padded number",
		label: "Insert counter",
		accessKey: "1",
		modifier: VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
		acceleratorKey: VirtualKey.I,
		icon: CreateIconFromGlyph((char)0xE8EF),
		execute: AddInsertCounterActionAsync);

	private async Task AddInsertCounterActionAsync()
	{
		InsertActionEditor actionEditor = new();
		actionEditor.Data.ValueSourceType = UserControls.InputControls.ValueSourceType.Counter;

		await EditAndAddJobItemAsync(actionEditor);
	}

	#endregion

	#region Add RemoveAction

	private AsyncUICommand _addRemoveActionCommand;
	public AsyncUICommand AddRemoveActionCommand => _addRemoveActionCommand ??= new(
		description: "Add an action that removes text",
		label: "Remove",
		accessKey: "R",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.R,
		icon: CreateIconFromSymbol(Symbol.Remove),
		execute: AddRemoveActionAsync);

	private async Task AddRemoveActionAsync()
	{
		await EditAndAddJobItemAsync(new RemoveActionEditor());
	}

	#endregion

	#region Add ReplaceAction

	private AsyncUICommand _addReplaceActionCommand;
	public AsyncUICommand AddReplaceActionCommand => _addReplaceActionCommand ??= new(
		description: "Add an action that replaces a text",
		label: "Replace",
		accessKey: "H",
		modifier: VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
		acceleratorKey: VirtualKey.R,
		icon: CreateIconFromSymbol(Symbol.Sync),
		execute: AddReplaceActionAsync);

	private async Task AddReplaceActionAsync()
	{
		await EditAndAddJobItemAsync(new ReplaceActionEditor());
	}

	#endregion

	#region Add ConvertCaseAction

	private AsyncUICommand _addConvertCaseActionCommand;
	public AsyncUICommand AddConvertCaseActionCommand => _addConvertCaseActionCommand ??= new(
		description: "Add an action that changes the casing of a text",
		label: "Change case",
		accessKey: "C",
		modifier: VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
		acceleratorKey: VirtualKey.C,
		icon: CreateIconFromSymbol(Symbol.Font),
		execute: AddConvertCaseActionAsync);

	private async Task AddConvertCaseActionAsync()
	{
		await EditAndAddJobItemAsync(new ChangeCaseActionEditor());
	}

	#endregion

	#region Add MoveStringAction

	private AsyncUICommand _addMoveStringActionCommand;
	public AsyncUICommand AddMoveStringActionCommand => _addMoveStringActionCommand ??= new(
		description: "Move a text around",
		label: "Move Text",
		accessKey: "M",
		modifier: VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
		acceleratorKey: VirtualKey.M,
		icon: CreateIconFromGlyph((char)0xE8AB),
		execute: AddMoveStringActionAsync);

	private async Task AddMoveStringActionAsync()
	{
		await EditAndAddJobItemAsync(new MoveStringActionEditor());
	}

	#endregion

	#endregion


	#region Helper functions

	private async Task<IJobItem> EditJobItemInDialogAsync(IActionEditor actionEditor)
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

	private async Task EditAndAddJobItemAsync(IActionEditor actionEditor)
	{
		IJobItem newAction = await EditJobItemInDialogAsync(actionEditor);

		if (newAction == null)
			return;

		ViewModel.Project.Jobs.Add(newAction);
		ViewModel.SelectedAction = newAction;
	}

	private static SymbolIconSource CreateIconFromSymbol(Symbol symbol)
	{
		return new() { Symbol = symbol };
	}

	private static FontIconSource CreateIconFromGlyph(char glyph)
	{
		return new() { Glyph = glyph.ToString() };
	}

	private void UpdateCommandStates()
	{
		MoveUpActionCommand.NotifyCanExecuteChanged();
		MoveDownActionCommand.NotifyCanExecuteChanged();
		EditActionCommand.NotifyCanExecuteChanged();
		DuplicateActionCommand.NotifyCanExecuteChanged();
		RemoveActionCommand.NotifyCanExecuteChanged();
		RemoveAllActionsCommand.NotifyCanExecuteChanged();
	}

	#endregion
}