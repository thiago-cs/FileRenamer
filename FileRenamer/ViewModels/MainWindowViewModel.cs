using System;
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
using FileRenamer.UserControls.ConditionalJobEditors;


namespace FileRenamer.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
	#region Fields

	private readonly MainWindow window;

	#endregion


	#region Project

	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (Project == null)
			return;

		Project.Jobs.CollectionChanged -= Jobs_CollectionChanged;
	}

	partial void OnProjectChanged(Project value)
	{
		UpdateCommandStates();

		if (Project == null)
			return;

		Project.Jobs.CollectionChanged += Jobs_CollectionChanged;
	}

	#endregion

	#region SelectedAction

	[ObservableProperty]
	private JobItem _selectedAction;

	partial void OnSelectedActionChanged(JobItem value)
	{
		UpdateCommandStates();
	}

	#endregion


	public MainWindowViewModel(MainWindow window)
	{
		Project = new();
		this.window = window;
	}


	#region Pick folder command

	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	[ObservableProperty]
	private IFolder _folder;

	partial void OnFolderChanged(IFolder value)
	{
		DoItCommand.NotifyCanExecuteChanged();
	}


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

	#endregion

	#region DoIt command

	[RelayCommand(CanExecute = nameof(CanDoIt))]
	private async Task DoItAsync()
	{
		try
		{
			await Project.RunAsync(Folder, CancellationToken.None);
		}
		catch (Exception ex)
		{
			Debugger.Log(0, "error", $"{ex.GetType().Name}: {ex.Message}");
		}
	}

	private bool CanDoIt()
	{
		return Folder != null && Project != null && Project.Jobs.Count != 0;
	}

	#endregion

	#region JobCollection event handlers

	private void Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		UpdateCommandStates();
	}

	#endregion

	#region Helper functions

	private async Task<JobItem> EditJobItemInDialogAsync<T>(IJobEditor<T> editor) where T : IJobEditorData
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

	private void UpdateCommandStates()
	{
		DoItCommand.NotifyCanExecuteChanged();
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