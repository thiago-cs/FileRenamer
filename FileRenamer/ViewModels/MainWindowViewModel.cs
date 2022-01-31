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

	public int SelectedIndex { get; set; }


	public MainWindowViewModel()
	{
		Project = new();
	}


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