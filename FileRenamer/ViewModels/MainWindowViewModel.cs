using FileRenamer.Core;
using FileRenamer.Core.Actions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;


namespace FileRenamer.ViewModels;

public sealed class MainWindowViewModel : BindableBase
{
	private Project _project;
	public Project Project
	{
		get => _project;
		set
		{
			if (_project != null)
				_project.Actions.CollectionChanged -= Actions_CollectionChanged;

			if (SetProperty(ref _project, value))
				UpdateTestOutput();

			if (_project != null)
				_project.Actions.CollectionChanged += Actions_CollectionChanged;
		}
	}

	private RenameActionBase _selectedAction;
	public RenameActionBase SelectedAction { get => _selectedAction; set => SetProperty(ref _selectedAction, value); }

	public int SelectedIndex { get; set; } = -1;


	public MainWindowViewModel()
	{
		Project = new();
	}


	#region Managing existing actions

	public void MoveSelectedActionUp()
	{
		if (SelectedIndex < 1)
		{
			// Oops!
			return;
		}

		ActionCollection actions = Project.Actions;
		int index = SelectedIndex;

		RenameActionBase previousAction = actions[index - 1];
		actions.RemoveAt(index - 1);
		actions.Insert(index, previousAction);
	}

	public void MoveSelectedActionDown()
	{
		if (SelectedIndex == -1 || Project.Actions.Count - 2 < SelectedIndex)
		{
			// Oops!
			return;
		}

		ActionCollection actions = Project.Actions;
		int index = SelectedIndex;

		RenameActionBase nextAction = actions[index + 1];
		actions.RemoveAt(index + 1);
		actions.Insert(index, nextAction);
	}

	public void DuplicateSelectedAction()
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		Project.Actions.Insert(SelectedIndex + 1, SelectedAction.Clone());
	}

	public void RemoveSelectedAction()
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		Project.Actions.RemoveAt(SelectedIndex);
	}

	public void RemoveAllActions()
	{
		Project.Actions.Clear();
	}

	#endregion

	#region CanExecute predicates

	private bool CanExecuteWhenActionsIsNotEmpty()
	{
		return Project.Actions.Count != 0;
	}

	private bool CanExecuteWhenSelectedActionIsNotNull()
	{
		return Project.Actions.Count != 0 && SelectedAction != null;
	}

	private bool CanExecuteWhenSelectedActionIsNotFirst()
	{
		return Project.Actions.Count != 0 && SelectedAction != null && SelectedAction != Project.Actions[0];
	}

	private bool CanExecuteWhenSelectedActionIsNotLast()
	{
		return Project.Actions.Count != 0 && SelectedAction != null && SelectedAction != Project.Actions[^1];
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
				UpdateTestOutput();
		}
	}

	private string _testOutput;
	public string TestOutput { get => _testOutput; private set => SetProperty(ref _testOutput, value); }

	private void UpdateTestOutput()
	{
		TestOutput = TestInput is not null
			? Project?.Actions.Run(TestInput)
			: null;
	}

	#endregion

	private void Actions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		// 1.
		UpdateTestOutput();

		// 2. 
		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Add:
				foreach (RenameActionBase action in e.NewItems)
					action.PropertyChanged += Action_PropertyChanged;
				break;

			case NotifyCollectionChangedAction.Remove:
				foreach (RenameActionBase action in e.OldItems)
					action.PropertyChanged -= Action_PropertyChanged;
				break;

			case NotifyCollectionChangedAction.Replace:
				foreach (RenameActionBase action in e.NewItems)
					action.PropertyChanged += Action_PropertyChanged;

				foreach (RenameActionBase action in e.OldItems)
					action.PropertyChanged -= Action_PropertyChanged;

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
	}
}