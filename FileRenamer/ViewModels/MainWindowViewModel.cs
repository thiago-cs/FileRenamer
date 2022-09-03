using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileRenamer.Core;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
	#region Fields

	private readonly Helpers.DelayedAction delayedUpdateTestOutput;
	private const int testOutputUpdateDelay = 400; // ms

	#endregion


	#region Properties

	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (value != null)
			value.Jobs.CollectionChanged -= Actions_CollectionChanged;
	}

	partial void OnProjectChanged(Project value)
	{
		if (value != null)
			value.Jobs.CollectionChanged += Actions_CollectionChanged;
	}

	[ObservableProperty]
	private IJobItem _selectedAction;

	public int SelectedIndex { get; set; } = -1;

	[ObservableProperty]
	private JobScope _scope;

	#endregion


	public MainWindowViewModel()
	{
		delayedUpdateTestOutput = new(UpdateTestOutput);
		Project = new();
	}


	#region Commands

	#region DoIt command

	[RelayCommand(CanExecute = nameof(CanDoIt))]
	private async Task DoItAsync()
	{
		try
		{
			await Project.RunAsync(CancellationToken.None);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debugger.Log(0, "error", $"{ex.GetType().Name}: {ex.Message}");
		}
	}

	private bool CanDoIt()
	{
		return Project.Folder != null && Project.Jobs.Count != 0;
	}

	#endregion

	#endregion

	#region Project actions management

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
	}

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
	}

	public void DuplicateSelectedAction()
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		if (SelectedAction is Core.Models.IDeepCopyable<RenameActionBase> copyable)
			Project.Jobs.Insert(SelectedIndex + 1, copyable.DeepCopy());
	}

	public void RemoveSelectedAction()
	{
		if (SelectedAction == null)
		{
			// Oops!
			return;
		}

		Project.Jobs.RemoveAt(SelectedIndex);
	}

	public void RemoveAllActions()
	{
		Project.Jobs.Clear();
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
		DoItCommand.NotifyCanExecuteChanged();

		// 2. 
		_ = delayedUpdateTestOutput.InvokeAsync(testOutputUpdateDelay);

		// 3. 
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

	#endregion
}