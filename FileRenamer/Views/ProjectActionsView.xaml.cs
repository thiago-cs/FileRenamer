using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.Conditionals;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.UserControls.ActionEditors;
using FileRenamer.UserControls.ConditionalJobEditors;
using FileRenamer.Models;


namespace FileRenamer.Views;

[ObservableObject]
public sealed partial class ProjectActionsView : UserControl
{
	#region Fields

	private string projectPath = null;

	#endregion


	#region Project property

	/*
	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (Project == null)
			return;

		Project.PropertyChanged -= Project_PropertyChanged;
		Project.Jobs.CollectionChanged -= Jobs_CollectionChanged;
	}

	partial void OnProjectChanged(Project value)
	{
		UpdateActionCommandStates();
		UpdateProjectCommandStates();

		if (Project == null)
			return;

		Project.PropertyChanged += Project_PropertyChanged;
		Project.Jobs.CollectionChanged += Jobs_CollectionChanged;
	}
	/*/
	public Project Project
	{
		get => (Project)GetValue(ProjectProperty);
		set => SetValue(ProjectProperty, value);
	}

	// Using a DependencyProperty as the backing store for Project.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty ProjectProperty =
		DependencyProperty.Register(
			nameof(Project),
			typeof(Project),
			typeof(ProjectActionsView),
			new PropertyMetadata(null, OnProjectChanged));

	private static void OnProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ProjectActionsView This)
			return;

		if (e.OldValue is Project oldProject)
		{
			oldProject.PropertyChanged -= This.Project_PropertyChanged;
			oldProject.Jobs.CollectionChanged -= This.Jobs_CollectionChanged;
		}

		This.UpdateProjectCommandStates();
		This.UpdateActionCommandStates();

		if (e.NewValue is Project newProject)
		{
			newProject.PropertyChanged += This.Project_PropertyChanged;
			newProject.Jobs.CollectionChanged += This.Jobs_CollectionChanged;
		}
	}
	//*/

	private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Project.HasUnsavedChanges))
			SaveProjectCommand.NotifyCanExecuteChanged();
	}

	private void Jobs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		UpdateActionCommandStates();
	}

	#endregion

	#region SelectedAction

	[ObservableProperty]
	private JobItem _selectedAction;

	partial void OnSelectedActionChanged(JobItem value)
	{
		UpdateActionCommandStates();
	}

	#endregion


	public ProjectActionsView()
	{
		NewProjectCommand = new()
		{
			Description = "Start a new project",
			Label = "New",
			AccessKey = "N",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Add)
		};
		NewProjectCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.N });
		NewProjectCommand.ExecuteRequested += NewProjectCommand_ExecuteRequested;

		OpenProjectCommand = new()
		{
			Description = "Open an existing project",
			Label = "Open",
			AccessKey = "O",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.OpenFile),
		};
		OpenProjectCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.O });
		OpenProjectCommand.ExecuteRequested += OpenProjectCommand_ExecuteRequested;

		SaveProjectCommand = new()
		{
			Description = "Save this project",
			Label = "Save",
			AccessKey = "S",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Save),
		};
		SaveProjectCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.S });
		SaveProjectCommand.ExecuteRequested += SaveProjectCommand_ExecuteRequested;
		SaveProjectCommand.CanExecuteRequested += SaveProjectCommand_CanExecuteRequested;

		SaveProjectAsCommand = new()
		{
			Description = "Save this project to a new file",
			Label = "Save as...",
			AccessKey = "B",
			IconSource = ExtendedUICommand.CreateIconSource('\uE792'),
		};
		SaveProjectAsCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, Key = VirtualKey.S });
		SaveProjectAsCommand.ExecuteRequested += SaveProjectAsCommand_ExecuteRequested;


		MoveUpActionCommand = new()
		{
			Description = "Move the selected action up",
			Label = "Move up",
			AccessKey = "U",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Up),
		};
		MoveUpActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Menu, Key = VirtualKey.Up });
		MoveUpActionCommand.ExecuteRequested += MoveSelectedActionUpCommand_ExecuteRequested;
		MoveUpActionCommand.CanExecuteRequested += MoveSelectedActionUpCommand_CanExecuteRequested;

		MoveDownActionCommand = new()
		{
			Description = "Move the selected action down",
			Label = "Move down",
			AccessKey = "D",
			IconSource = ExtendedUICommand.CreateIconSource('\uE74B'),
		};
		MoveDownActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Menu, Key = VirtualKey.Down });
		MoveDownActionCommand.ExecuteRequested += MoveSelectedActionDownCommand_ExecuteRequested;
		MoveDownActionCommand.CanExecuteRequested += MoveSelectedActionDownCommand_CanExecuteRequested;

		EditActionCommand = new()
		{
			Description = "Edit the selected action",
			Label = "Edit",
			AccessKey = "T",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Edit),
		};
		EditActionCommand.ExecuteRequested += EditSelectedActionCommand_ExecuteRequested;
		EditActionCommand.CanExecuteRequested += EditSelectedActionCommand_CanExecuteRequested;

		DuplicateActionCommand = new()
		{
			Description = "Duplicate the selected action",
			Label = "Duplicate",
			AccessKey = "V",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Copy),
		};
		DuplicateActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.D });
		DuplicateActionCommand.ExecuteRequested += DuplicateSelectedActionCommand_ExecuteRequested;
		DuplicateActionCommand.CanExecuteRequested += DuplicateSelectedActionCommand_CanExecuteRequested;

		RemoveActionCommand = new()
		{
			Description = "Remove the selected action",
			Label = "Remove",
			AccessKey = "Del",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Delete),
		};
		RemoveActionCommand.ExecuteRequested += RemoveSelectedActionCommand_ExecuteRequested;
		RemoveActionCommand.CanExecuteRequested += RemoveSelectedActionCommand_CanExecuteRequested;

		RemoveAllActionsCommand = new()
		{
			Description = "Remove all actions",
			Label = "Clear",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Clear),
		};
		RemoveAllActionsCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.Delete });
		RemoveAllActionsCommand.ExecuteRequested += RemoveAllActionsCommand_ExecuteRequested;
		RemoveAllActionsCommand.CanExecuteRequested += RemoveAllActionsCommand_CanExecuteRequested;


		AddInsertActionCommand = new()
		{
			Description = "Add an action that inserts a text",
			Label = "Insert",
			AccessKey = "I",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Add),
		};
		AddInsertActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.I });
		AddInsertActionCommand.ExecuteRequested += AddInsertActionCommand_ExecuteRequested;

		AddInsertCounterActionCommand = new()
		{
			Description = "Add an action that inserts a padded number",
			Label = "Insert counter",
			AccessKey = "1",
			IconSource = ExtendedUICommand.CreateIconSource('\uE8EF'),
		};
		AddInsertCounterActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, Key = VirtualKey.I });
		AddInsertCounterActionCommand.ExecuteRequested += AddInsertCounterActionCommand_ExecuteRequested;

		AddRemoveActionCommand = new()
		{
			Description = "Add an action that removes text",
			Label = "Remove",
			AccessKey = "R",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Remove),
		};
		AddRemoveActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.R });
		AddRemoveActionCommand.ExecuteRequested += AddRemoveActionCommand_ExecuteRequested;

		AddReplaceActionCommand = new()
		{
			Description = "Add an action that replaces a text",
			Label = "Replace",
			AccessKey = "H",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Sync),
		};
		AddReplaceActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control, Key = VirtualKey.H });
		AddReplaceActionCommand.ExecuteRequested += AddReplaceActionCommand_ExecuteRequested;

		AddConvertCaseActionCommand = new()
		{
			Description = "Add an action that changes the casing of a text",
			Label = "Change case",
			AccessKey = "C",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Font),
		};
		AddConvertCaseActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, Key = VirtualKey.C });
		AddConvertCaseActionCommand.ExecuteRequested += AddConvertCaseActionCommand_ExecuteRequested;

		AddMoveStringActionCommand = new()
		{
			Description = "Move a text around",
			Label = "Move Text",
			AccessKey = "M",
			IconSource = ExtendedUICommand.CreateIconSource('\uE8AB'),
		};
		AddMoveStringActionCommand.KeyboardAccelerators.Add(new() { Modifiers = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, Key = VirtualKey.M });
		AddMoveStringActionCommand.ExecuteRequested += AddMoveStringActionCommand_ExecuteRequested;


		AddConditionalCommand = new()
		{
			Description = "Add a name pattern check",
			Label = "Name pattern",
			IconSource = ExtendedUICommand.CreateIconSource('\uEA38'),
		};
		AddConditionalCommand.ExecuteRequested += AddConditionalCommand_ExecuteRequested;


		InitializeComponent();
	}


	#region Project Commands

	public ExtendedUICommand NewProjectCommand { get; private init; }
	public ExtendedUICommand OpenProjectCommand { get; private init; }
	public ExtendedUICommand SaveProjectCommand { get; private init; }
	public ExtendedUICommand SaveProjectAsCommand { get; private init; }

	private async void NewProjectCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		// 1.
		bool changesWereSaved = await ShowSaveChangesConfirmationDialogAsync();

		if (!changesWereSaved)
			return;

		// 2.
		Project = new();
	}

	private async void OpenProjectCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
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

		picker.SetOwnerWindow(GetMainWindow());

		var file = await picker.PickSingleFileAsync();

		if (file == null)
		{
			//
			return;
		}

		// 3.
		try
		{
			using Stream stream = await file.OpenStreamForWriteAsync();
			using StreamReader input = new(stream);
			Project = await Project.ReadXmlAsync(input);
			projectPath = file.Path;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			throw;
		}
	}

	private void SaveProjectCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		if (!Project.HasUnsavedChanges)
			return;

		_ = SaveProjectAsync();
	}

	private void SaveProjectCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = Project != null && Project.HasUnsavedChanges;
	}

	private void SaveProjectAsCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		_ = SaveProjectAsync();
	}

	private async Task SaveProjectAsync()
	{
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

				savePicker.SetOwnerWindow(GetMainWindow());

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
			projectPath = file.Path;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.ToString());
			throw;
		}
	}

	#endregion

	#region Manage actions commands

	public ExtendedUICommand MoveUpActionCommand { get; private init; }
	public ExtendedUICommand MoveDownActionCommand { get; private init; }
	public ExtendedUICommand EditActionCommand { get; private init; }
	public ExtendedUICommand DuplicateActionCommand { get; private init; }
	public ExtendedUICommand RemoveActionCommand { get; private init; }
	public ExtendedUICommand RemoveAllActionsCommand { get; private init; }

	private void MoveSelectedActionUpCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		JobCollection jobs = GetOwningJobCollection(SelectedAction);

		if (jobs == null)
		{
			// Oops!
			return;
		}

		int index = jobs.IndexOf(SelectedAction);

		if (index < 1)
		{
			// Oops!
			return;
		}

		//jobs.Move(index, index - 1);
		JobItem previousJob = jobs[index - 1];
		jobs.RemoveAt(index - 1);
		jobs.Insert(index, previousJob);
	}

	private void MoveSelectedActionUpCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = CanExecuteWhenSelectedActionIsNotFirst();
	}

	private void MoveSelectedActionDownCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		JobCollection jobs = GetOwningJobCollection(SelectedAction);

		if (jobs == null)
		{
			// Oops!
			return;
		}

		int index = jobs.IndexOf(SelectedAction);

		if (index == -1 || jobs.Count - 2 < index)
		{
			// Oops!
			return;
		}

		//jobs.Move(index, index + 1);
		JobItem nextJob = jobs[index + 1];
		jobs.RemoveAt(index + 1);
		jobs.Insert(index, nextJob);
	}

	private void MoveSelectedActionDownCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = CanExecuteWhenSelectedActionIsNotLast();
	}

	private async void EditSelectedActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		//
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		JobCollection jobs = GetOwningJobCollection(SelectedAction);

		if (jobs == null)
		{
			// Oops!
			return;
		}

		//
		IJobEditor<IJobEditorData> jobEditor = SelectedAction switch
		{
			InsertAction job => new InsertRenameJobEditor(job),
			RemoveAction job => new RemoveRenameJobEditor(job),
			ReplaceAction job => new ReplaceRenameJobEditor(job),
			ChangeRangeCaseAction job => new ChangeCaseRenameJobEditor(job),
			ChangeStringCaseAction job => new ChangeCaseRenameJobEditor(job),
			MoveStringAction job => new MoveStringRenameJobEditor(job),

			ItemNameJobConditional job => new NamePatternConditionalJobEditor(job),

			_ => null,
		};

		//
		JobItem item = await EditJobItemInDialogAsync(jobEditor);

		if (item == null)
			return;

		int index = jobs.IndexOf(SelectedAction);
		jobs.RemoveAt(index);
		jobs.Insert(index, item);
	}

	private void EditSelectedActionCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = CanExecuteWhenSelectedActionIsNotNull();
	}

	private void DuplicateSelectedActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		JobCollection jobs = GetOwningJobCollection(SelectedAction);

		if (jobs == null)
		{
			// Oops!
			return;
		}

		int index = jobs.IndexOf(SelectedAction);

		if (index == -1)
		{
			// Oops!
			return;
		}

		jobs.Insert(index + 1, SelectedAction.DeepCopy());
	}

	private void DuplicateSelectedActionCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = CanExecuteWhenSelectedActionIsNotNull();
	}

	private void RemoveSelectedActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		JobCollection jobs = GetOwningJobCollection(SelectedAction);

		if (jobs == null)
		{
			// Oops!
			return;
		}

		jobs.Remove(SelectedAction);
	}

	private void RemoveSelectedActionCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = CanExecuteWhenSelectedActionIsNotNull();
	}

	private void RemoveAllActionsCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		Project.Jobs.Clear();
	}

	private void RemoveAllActionsCommand_CanExecuteRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
	{
		args.CanExecute = CanExecuteWhenActionsIsNotEmpty();
	}

	#endregion

	#region Add actions commands

	public ExtendedUICommand AddInsertActionCommand { get; private init; }
	public ExtendedUICommand AddInsertCounterActionCommand { get; private init; }
	public ExtendedUICommand AddRemoveActionCommand { get; private init; }
	public ExtendedUICommand AddReplaceActionCommand { get; private init; }
	public ExtendedUICommand AddConvertCaseActionCommand { get; private init; }
	public ExtendedUICommand AddMoveStringActionCommand { get; private init; }

	private async void AddInsertActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		await EditAndAddJobItemAsync(new InsertRenameJobEditor());
	}

	private async void AddInsertCounterActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		InsertRenameJobEditor jobEditor = new();
		jobEditor.Data.ValueSourceType = UserControls.InputControls.ValueSourceType.Counter;

		await EditAndAddJobItemAsync(jobEditor);
	}

	private async void AddRemoveActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		await EditAndAddJobItemAsync(new RemoveRenameJobEditor());
	}

	private async void AddReplaceActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		await EditAndAddJobItemAsync(new ReplaceRenameJobEditor());
	}

	private async void AddConvertCaseActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		await EditAndAddJobItemAsync(new ChangeCaseRenameJobEditor());
	}

	private async void AddMoveStringActionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		await EditAndAddJobItemAsync(new MoveStringRenameJobEditor());
	}

	#endregion

	#region Add conditional item commands

	public ExtendedUICommand AddConditionalCommand { get; private init; }

	private async void AddConditionalCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		NamePatternConditionalJobEditor jobEditor = new();

		await EditAndAddJobItemAsync(jobEditor);
	}

	#endregion

	#region Command Helpers

	private void UpdateProjectCommandStates()
	{
		SaveProjectCommand.NotifyCanExecuteChanged();
	}

	private void UpdateActionCommandStates()
	{
		MoveUpActionCommand.NotifyCanExecuteChanged();
		MoveDownActionCommand.NotifyCanExecuteChanged();
		EditActionCommand.NotifyCanExecuteChanged();
		DuplicateActionCommand.NotifyCanExecuteChanged();
		AddRemoveActionCommand.NotifyCanExecuteChanged();
		RemoveAllActionsCommand.NotifyCanExecuteChanged();
	}

	public bool CanExecuteWhenActionsIsNotEmpty()
	{
		return Project != null && Project.Jobs.Count != 0;
	}

	public bool CanExecuteWhenSelectedActionIsNotNull()
	{
		return Project != null && /*Project.Jobs.Count != 0 &&*/ SelectedAction != null;
	}

	public bool CanExecuteWhenSelectedActionIsNotFirst()
	{
		return Project != null
			&& SelectedAction != null
			&& GetOwningJobCollection(SelectedAction) is JobCollection jobs
			&& jobs.Count != 0
			&& SelectedAction != jobs[0];
	}

	public bool CanExecuteWhenSelectedActionIsNotLast()
	{
		return Project != null
			&& SelectedAction != null
			&& GetOwningJobCollection(SelectedAction) is JobCollection jobs
			&& jobs.Count != 0
			&& SelectedAction != jobs[^1];
	}

	#endregion

	#region Helper functions

	private static MainWindow GetMainWindow()
	{
		return (Microsoft.UI.Xaml.Application.Current as App).Window;
	}

	/// <summary>
	/// Shows a confirmation dialog when there are unsaved changes in the project.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if there are no unsaved changes or the changes were successfully saved.
	/// <see langword="false"/> if there are changes that could not be saved (e.g. the user canceled the operation).
	/// </returns>
	private async Task<bool> ShowSaveChangesConfirmationDialogAsync()
	{
		if (!Project.HasUnsavedChanges)
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
			result = await GetMainWindow().ShowDialogAsync(dialog);
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

				if (Project.HasUnsavedChanges)
					return false;
				break;

			// Don't save
			case ContentDialogResult.Secondary:
				break;
		}

		return true;
	}

	private static async Task<JobItem> EditJobItemInDialogAsync<T>(IJobEditor<T> editor) where T : IJobEditorData
	{
		if (editor == null)
		{
			// Oops!
			return null;
		}

		//
		ContentDialogResult result = await GetMainWindow().ShowJobEditorDialogAsync(editor);

		if (result != ContentDialogResult.Primary)
			return null;

		if (editor.Data.HasErrors)
		{
			// Oops!
			return null;
		}

		try
		{
			return editor.Data.GetJobItem();
		}
#pragma warning disable CS0168 // Variable is declared but never used
#pragma warning disable IDE0059 // Unnecessary assignment of a value
		catch (Exception ex)
#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0168 // Variable is declared but never used
		{
			throw;
		}
	}

	private async Task EditAndAddJobItemAsync<T>(IJobEditor<T> jobEditor) where T : IJobEditorData
	{
		JobItem newAction = await EditJobItemInDialogAsync(jobEditor);

		if (newAction == null)
			return;

		if (SelectedAction is ComplexJobItem complexItem)
			complexItem.Jobs.Add(newAction);
		else
		{
			JobCollection jobs = GetOwningJobCollection(SelectedAction) ?? Project.Jobs;
			jobs.Add(newAction);
		}

		SelectedAction = newAction;
	}

	private JobCollection GetOwningJobCollection(JobItem job)
	{
		return job == null
				? null
			 : job.OwningJobCollectionReference == null
				? Project.Jobs
			 : job.OwningJobCollectionReference.TryGetTarget(out JobCollection jobs)
				? jobs
				: null;
	}

	#endregion
}