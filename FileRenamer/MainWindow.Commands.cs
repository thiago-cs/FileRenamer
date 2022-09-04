using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Models;
using FileRenamer.UserControls.ActionEditors;


namespace FileRenamer;

partial class MainWindow
{
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
	public readonly UICommand RemoveAllActionsCommand;


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
			MoveStringAction action => new MoveStringActionEditor(action),
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

	private async void AddMoveStringAction()
	{
		await EditAndAddJobItem(new MoveStringActionEditor());
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
		RemoveAllActionsCommand.NotifyCanExecuteChanged();
	}
}