using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileRenamer.Core;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.Conditionals;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Models;
using FileRenamer.UserControls.ActionEditors;


namespace FileRenamer.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
	#region Fields

	private readonly MainWindow window;
	private readonly Helpers.DelayedAction delayedUpdateTestOutput;
	private const int testOutputUpdateDelay = 400; // ms

	#endregion


	#region Properties

	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (value != null)
		{
			value.PropertyChanged -= Project_PropertyChanged;
			value.Jobs.CollectionChanged -= Actions_CollectionChanged;
		}
	}

	partial void OnProjectChanged(Project value)
	{
		if (value != null)
		{
			value.PropertyChanged += Project_PropertyChanged;
			value.Jobs.CollectionChanged += Actions_CollectionChanged;
			UpdateCommandStates();
		}
	}

	private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Project.Scope))
			UpdatePreview();
	}

	[ObservableProperty]
	private JobItem _selectedAction;

	partial void OnSelectedActionChanged(JobItem value)
	{
		UpdateCommandStates();
	}

	public int SelectedIndex { get; set; } = -1;

	#endregion


	public MainWindowViewModel(MainWindow window)
	{
		delayedUpdateTestOutput = new(UpdateTestOutput);
		Project = new();
		this.window = window;
	}


	#region Project Commands

	private string projectPath = null;

	[ObservableProperty]
	private bool _hasUnsavedChanges = false;

	partial void OnHasUnsavedChangesChanged(bool value)
	{
		SaveProjectCommand.NotifyCanExecuteChanged();
	}

	[ObservableProperty]
	private bool _autoSavedChanges = false;


	#region New Project

	private AsyncUICommand _newProjectCommand;
	public AsyncUICommand NewProjectCommand => _newProjectCommand ??= new(
		description: "Start a new project",
		label: "New",
		accessKey: "N",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.N,
		icon: CreateIconFromSymbol(Symbol.Add),
		execute: NewProject);

	public async Task NewProject()
	{
		// 1.
		bool changesWereSaved = await ShowSaveChangesConfirmationDialogAsync();

		if (!changesWereSaved)
			return;

		// 2.
		Project = new();
		HasUnsavedChanges = false;
	}

	#endregion

	#region Load Project

	private AsyncUICommand _loadProjectCommand;
	public AsyncUICommand LoadProjectCommand => _loadProjectCommand ??= new(
		description: "Load an existing project",
		label: "Load",
		accessKey: "L",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.O,
		icon: CreateIconFromSymbol(Symbol.OpenLocal),
		execute: LoadProject);

	public async Task LoadProject()
	{
		// 1.
		bool changesWereSaved = await ShowSaveChangesConfirmationDialogAsync();

		if (!changesWereSaved)
			return;

		// 2.
		FileOpenPicker picker = new()
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
			ViewMode = PickerViewMode.List,
			FileTypeFilter = { ".xml", },
		};

		picker.SetOwnerWindow(window);

		var file = await picker.PickSingleFileAsync();

		if (file == null)
		{
			//
			return;
		}

		// 3.
		try
		{
			using Stream stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false);
			using StreamReader input = new(stream);
			Project = await Project.ReadXmlAsync(input).ConfigureAwait(false);
			HasUnsavedChanges = false;
			projectPath = file.Path;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			throw;
		}
	}

	#endregion

	#region Save Project

	private AsyncUICommand _saveProjectCommand;
	public AsyncUICommand SaveProjectCommand => _saveProjectCommand ??= new(
		description: "Save this project",
		label: "Save",
		accessKey: "S",
		modifier: VirtualKeyModifiers.Control,
		acceleratorKey: VirtualKey.S,
		icon: CreateIconFromSymbol(Symbol.Save),
		execute: SaveProjectAsync,
		canExecute: CanSaveProject);


	public async Task SaveProjectAsync()
	{
		if (!HasUnsavedChanges)
			return;

		Windows.Storage.StorageFile file;

		try
		{
			if (projectPath != null)
				file = await Windows.Storage.StorageFile.GetFileFromPathAsync(projectPath);
			else
			{
				FileSavePicker savePicker = new()
				{
					SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
					SuggestedFileName = "my file rename project",
					FileTypeChoices = { { "XML file", new[] { ".xml" } } },
				};

				savePicker.SetOwnerWindow(window);

				file = await savePicker.PickSaveFileAsync();
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.ToString());
			throw;
		}

		if (file == null)
		{
			//
			return;
		}

		try
		{
			using Stream stream = await file.OpenStreamForWriteAsync();
			await Project.WriteXmlAsync(stream).ConfigureAwait(false);
			HasUnsavedChanges = false;
			projectPath = file.Path;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			throw;
		}
	}

	public bool CanSaveProject()
	{
		return HasUnsavedChanges;
	}

	/// <summary>
	/// Shows a confirmation dialog when there are unsaved changes in the project.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if there are no unsaved changes or the changes were successfully saved.
	/// <see langword="false"/> if there are changes that could not be saved (e.g. the user canceled the operation).</returns>
	private async Task<bool> ShowSaveChangesConfirmationDialogAsync()
	{
		if (!HasUnsavedChanges)
			return true;

		// 1.
		ContentDialog dialog = new()
		{
			Title = "Save your changes to this project?",
			Content = "You have unsaved changes in this project. Do you want to save them?",
			PrimaryButtonText = "Save",
			SecondaryButtonText = "Don't save",
			CloseButtonText = "Cancel",
			DefaultButton = ContentDialogButton.Primary,
		};

		ContentDialogResult result;

		try
		{
			result = await window.ShowDialogAsync(dialog);
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			throw;
		}

		// 2.
		switch (result)
		{
			// Cancel
			case ContentDialogResult.None:
				return false;

			// Save
			case ContentDialogResult.Primary:
				await SaveProjectAsync();

				if (HasUnsavedChanges)
					return false;
				break;

			// Don't save
			case ContentDialogResult.Secondary:
				break;
		}

		return true;
	}

	#endregion

	#endregion

	#region Manage actions commands

	#region Move Up

	private UICommand _moveUpActionCommand;
	public UICommand MoveUpActionCommand => _moveUpActionCommand ??= new(
		description: "Move the selected action up",
		label: "Move up",
		accessKey: "U",
		modifier: VirtualKeyModifiers.Menu,
		acceleratorKey: VirtualKey.Up,
		icon: CreateIconFromSymbol(Symbol.Up),
		execute: MoveSelectedActionUp,
		canExecute: CanExecuteWhenSelectedActionIsNotFirst);

	public void MoveSelectedActionUp()
	{
		if (SelectedIndex < 1)
		{
			// Oops!
			return;
		}

		JobCollection actions = Project.Jobs;
		int index = SelectedIndex;

		var previousAction = actions[index - 1];
		actions.RemoveAt(index - 1);
		actions.Insert(index, previousAction);

		HasUnsavedChanges = true;
	}

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
		execute: MoveSelectedActionDown,
		canExecute: CanExecuteWhenSelectedActionIsNotLast);

	public void MoveSelectedActionDown()
	{
		if (SelectedIndex == -1 || Project.Jobs.Count - 2 < SelectedIndex)
		{
			// Oops!
			return;
		}

		JobCollection actions = Project.Jobs;
		int index = SelectedIndex;

		var nextAction = actions[index + 1];
		actions.RemoveAt(index + 1);
		actions.Insert(index, nextAction);

		HasUnsavedChanges = true;
	}

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
		canExecute: CanExecuteWhenSelectedActionIsNotNull);

	private async Task EditSelectedActionAsync()
	{
		//
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		//
		IJobEditor jobEditor = SelectedAction switch
		{
			InsertAction job => new InsertRenameJobEditor(job),
			RemoveAction job => new RemoveRenameJobEditor(job),
			ReplaceAction job => new ReplaceRenameJobEditor(job),
			ChangeRangeCaseAction job => new ChangeCaseRenameJobEditor(job),
			ChangeStringCaseAction job => new ChangeCaseRenameJobEditor(job),
			MoveStringAction job => new MoveStringRenameJobEditor(job),

			_ => null,
		};

		//
		JobItem item = await EditJobItemInDialogAsync(jobEditor);

		if (item == null)
			return;

		int index = Project.Jobs.IndexOf(SelectedAction);
		Project.Jobs.RemoveAt(index);
		Project.Jobs.Insert(index, item);

		HasUnsavedChanges = true;
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
		execute: DuplicateSelectedAction,
		canExecute: CanExecuteWhenSelectedActionIsNotNull);

	public void DuplicateSelectedAction()
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		if (SelectedAction is Core.Models.IDeepCopyable<RenameFileJob> copyable)
		{
			Project.Jobs.Insert(SelectedIndex + 1, copyable.DeepCopy());

			HasUnsavedChanges = true;
		}
	}

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
		execute: RemoveSelectedAction,
		canExecute: CanExecuteWhenSelectedActionIsNotNull);

	public void RemoveSelectedAction()
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		Project.Jobs.RemoveAt(SelectedIndex);

		HasUnsavedChanges = true;
	}

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
		execute: RemoveAllActions,
		canExecute: CanExecuteWhenActionsIsNotEmpty);

	public void RemoveAllActions()
	{
		Project.Jobs.Clear();

		HasUnsavedChanges = true;
	}

	#endregion

	#endregion

	#region Add actions commands

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
		await EditAndAddJobItemAsync(new InsertRenameJobEditor());
		HasUnsavedChanges = true;
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
		InsertRenameJobEditor jobEditor = new();
		jobEditor.Data.ValueSourceType = UserControls.InputControls.ValueSourceType.Counter;

		await EditAndAddJobItemAsync(jobEditor);
		HasUnsavedChanges = true;
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
		await EditAndAddJobItemAsync(new RemoveRenameJobEditor());
		HasUnsavedChanges = true;
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
		await EditAndAddJobItemAsync(new ReplaceRenameJobEditor());
		HasUnsavedChanges = true;
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
		await EditAndAddJobItemAsync(new ChangeCaseRenameJobEditor());
		HasUnsavedChanges = true;
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
		await EditAndAddJobItemAsync(new MoveStringRenameJobEditor());
		HasUnsavedChanges = true;
	}

	#endregion

	#endregion

	#region Add conditional item commands

	#region Add ItemNameJobConditional

	private AsyncUICommand _addConditionalCommand;
	public AsyncUICommand AddConditionalCommand => _addConditionalCommand ??= new(
		description: "Add a name pattern check",
		label: "name pattern",
		accessKey: "",
		modifier: null,
		acceleratorKey: null,
		icon: CreateIconFromSymbol(Symbol.Street),
		execute: AddConditionalAsync);

	private async Task AddConditionalAsync()
	{
		await Task.CompletedTask;
		var conditional = new ItemNameJobConditional("\\d{3,4}", false);
		Project.Jobs.Add(conditional);
		HasUnsavedChanges = true;
	}

	#endregion

	#endregion

	#region Pick folder command

	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	[ObservableProperty]
	private IFolder _folder;

	partial void OnFolderChanged(IFolder value)
	{
		if (value != null)
		{
			DoItCommand.NotifyCanExecuteChanged();

			_ = UpdateItemsInFolderAsync();
		}
	}

	private List<IItem> itemsInFolder;


	[RelayCommand]
	private async Task PickFolderAsync()
	{
		// 1. 
		FolderPicker picker = new()
		{
			SuggestedStartLocation = PickerLocationId.Downloads,
			ViewMode = PickerViewMode.List,
			FileTypeFilter = { "*", },
		};

		// Make folder Picker work in Win32
		picker.SetOwnerWindow(window);

		// Use file picker like normal!
		Windows.Storage.StorageFolder folder = await picker.PickSingleFolderAsync();

		// 2. 
		if (folder == null)
			return;

		Folder = new Folder(folder);
	}

	private async Task UpdateItemsInFolderAsync()
	{
		itemsInFolder = new();
		itemsInFolder.AddRange(await Folder.GetSubfoldersAsync());
		itemsInFolder.AddRange(await Folder.GetFilesAsync());

		UpdatePreview();
	}

	#endregion

	#region Result preview

	[ObservableProperty]
	private IList<JobTarget> _preview;

	[ObservableProperty]
	private bool _showLivePreview = false;

	partial void OnShowLivePreviewChanged(bool value)
	{
		UpdatePreview();
	}

	private void UpdatePreview()
	{
		if (ShowLivePreview)
			Preview = itemsInFolder == null ? null : (IList<JobTarget>)Project.ComputeChanges(itemsInFolder);
	}

	#endregion

	#region DoIt command

	[RelayCommand(CanExecute = nameof(CanDoIt))]
	private async Task DoItAsync()
	{
		try
		{
			await Project.RunAsync(Folder, CancellationToken.None);

			await UpdateItemsInFolderAsync();
		}
		catch (Exception ex)
		{
			Debugger.Log(0, "error", $"{ex.GetType().Name}: {ex.Message}");
		}
	}

	private bool CanDoIt()
	{
		return Folder != null && Project.Jobs.Count != 0;
	}

	#endregion

	#region CanExecute predicates

	public bool CanExecuteWhenActionsIsNotEmpty()
	{
		return Project.Jobs.Count != 0;
	}

	public bool CanExecuteWhenSelectedActionIsNotNull()
	{
		return Project.Jobs.Count != 0 && SelectedAction != null;
	}

	public bool CanExecuteWhenSelectedActionIsNotFirst()
	{
		return Project.Jobs.Count != 0 && SelectedAction != null && SelectedAction != Project.Jobs[0];
	}

	public bool CanExecuteWhenSelectedActionIsNotLast()
	{
		return Project.Jobs.Count != 0 && SelectedAction != null && SelectedAction != Project.Jobs[^1];
	}

	#endregion

	#region Test lab

	private string _testInput;
	public string TestInput
	{
		get => _testInput;
		set
		{
			if (SetProperty(ref _testInput, value))
				_ = delayedUpdateTestOutput.InvokeAsync(testOutputUpdateDelay);
		}
	}

	private string _testOutput;
	public string TestOutput { get => _testOutput; private set => SetProperty(ref _testOutput, value); }

	internal void ImmidiatelyTestInput(string name)
	{
		TestInput = name;
		UpdateTestOutput();
	}

	private void UpdateTestOutput()
	{
		if (string.IsNullOrEmpty(TestInput) || Project == null)
		{
			TestOutput = string.Empty;
			return;
		}

		Project.Jobs.Reset();

		JobTarget target = new(new Core.FileSystem.FileMock(TestInput), 0);
		JobContext context = new(Project.Jobs, new[] { target });

		Project.Jobs.Run(target, context);
		TestOutput = target.NewFileName;
	}

	#endregion

	#region Action and ActionCollection event handlers

	private void Actions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		// 1.
		UpdateCommandStates();
		UpdatePreview();

		// 2. 
		_ = delayedUpdateTestOutput.InvokeAsync(testOutputUpdateDelay);

		// 3. 
		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Add:
				foreach (JobItem item in e.NewItems)
					item.PropertyChanged += Action_PropertyChanged;
				break;

			case NotifyCollectionChangedAction.Remove:
				foreach (JobItem item in e.OldItems)
					item.PropertyChanged -= Action_PropertyChanged;
				break;

			case NotifyCollectionChangedAction.Replace:
				foreach (JobItem item in e.NewItems)
					item.PropertyChanged += Action_PropertyChanged;

				foreach (JobItem item in e.OldItems)
					item.PropertyChanged -= Action_PropertyChanged;

				break;

			case NotifyCollectionChangedAction.Reset:
			case NotifyCollectionChangedAction.Move:
			default:
				break;
		}
	}

	private void Action_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		UpdateTestOutput();
		UpdatePreview();

		HasUnsavedChanges = true;
	}

	#endregion

	#region Helper functions

	private async Task<JobItem> EditJobItemInDialogAsync(IJobEditor editor)
	{
		if (editor == null)
		{
			// Oops!
			return null;
		}

		//
		ContentDialogResult result = await window.ShowJobEditorDialogAsync(editor);

		if (result != ContentDialogResult.Primary)
			return null;

		if (!editor.IsValid)
		{
			// Oops!
			return null;
		}

		return editor.GetRenameAction();
	}

	private async Task EditAndAddJobItemAsync(IJobEditor jobEditor)
	{
		JobItem newAction = await EditJobItemInDialogAsync(jobEditor);

		if (newAction == null)
			return;

		if (SelectedAction is ComplexJobItem complexItem)
			complexItem.Jobs.Add(newAction);
		else
			Project.Jobs.Add(newAction);

		SelectedAction = newAction;
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
		//
		MoveUpActionCommand.NotifyCanExecuteChanged();
		MoveDownActionCommand.NotifyCanExecuteChanged();
		EditActionCommand.NotifyCanExecuteChanged();
		DuplicateActionCommand.NotifyCanExecuteChanged();
		RemoveActionCommand.NotifyCanExecuteChanged();
		RemoveAllActionsCommand.NotifyCanExecuteChanged();

		//
		DoItCommand.NotifyCanExecuteChanged();
	}

	#endregion
}