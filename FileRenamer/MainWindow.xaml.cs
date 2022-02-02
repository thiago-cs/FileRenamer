﻿using System;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Models;
using FileRenamer.ViewModels;


namespace FileRenamer;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
	internal MainWindowViewModel ViewModel { get; } = new();


	public MainWindow()
	{
		// 1. 
		InitializeComponent();

		#region 2. Commands

		const VirtualKeyModifiers Control_Shift = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift;

		// 2.1. Project commands
		NewProjectCommand = CreateCommand2("New", "N", Symbol.Add, VirtualKeyModifiers.Control, VirtualKey.N, ExecuteNewProject);
		LoadProjectCommand = CreateCommand2("Load", "L", Symbol.OpenLocal, VirtualKeyModifiers.Control, VirtualKey.O, ExecuteLoadProject);
		SaveProjectCommand = CreateCommand2("Save", "S", Symbol.Save, VirtualKeyModifiers.Control, VirtualKey.S, ExecuteSaveProject);

		// 2.2. Manage existing actions commands
		MoveUpActionCommand = CreateCommand2("Move up", "U", Symbol.Up, VirtualKeyModifiers.Menu, VirtualKey.Up, ViewModel.MoveSelectedActionUp, CanExecuteWhenSelectedActionIsNotFirst);
		MoveDownActionCommand = CreateCommand3("Move down", "D", (char)0xE74B, VirtualKeyModifiers.Menu, VirtualKey.Down, ViewModel.MoveSelectedActionDown, CanExecuteWhenSelectedActionIsNotLast);
		EditActionCommand = CreateCommand2("Edit", "T", Symbol.Edit, null, VirtualKey.F2, ExecuteEditAction, CanExecuteWhenSelectedActionIsNotNull);
		DuplicateActionCommand = CreateCommand2("Duplicate", "V", Symbol.Copy, VirtualKeyModifiers.Control, VirtualKey.D, ViewModel.DuplicateSelectedAction, CanExecuteWhenSelectedActionIsNotNull);
		RemoveActionCommand = CreateCommand2("Remove", "Del", Symbol.Delete, null, VirtualKey.Delete, ViewModel.RemoveSelectedAction, CanExecuteWhenSelectedActionIsNotNull);
		RemoveAllActionCommand = CreateCommand2("Clear", "", Symbol.Clear, VirtualKeyModifiers.Control, VirtualKey.Delete, ViewModel.RemoveAllActions);

		// 2.3. Add new actions commands
		AddInsertActionCommand = CreateCommand2("Insert", "I", Symbol.Add, VirtualKeyModifiers.Control, VirtualKey.I, ExecuteAddInsertAction);
		AddRemoveActionCommand = CreateCommand2("Remove", "R", Symbol.Remove, VirtualKeyModifiers.Control, VirtualKey.R, ExecuteAddRemoveAction);
		AddInsertCounterActionCommand = CreateCommand3("Insert counter", "1", (char)0xE8EF, Control_Shift, VirtualKey.I, ExecuteAddInsertCounterAction);
		AddReplaceActionCommand = CreateCommand2("Replace", "H", Symbol.Sync, Control_Shift, VirtualKey.R, ExecuteAddReplaceAction);
		AddConvertCaseActionCommand = CreateCommand2("Convert case", "C", Symbol.Font, Control_Shift, VirtualKey.C, ExecuteAddConvertCaseAction);

		#endregion


		// 3.
		ViewModel.PropertyChanged += ViewModel_PropertyChanged;



		#region Local functions

		static UICommand CreateCommand(string label, string accessKey, IconSource icon, VirtualKeyModifiers? modifier, VirtualKey acceleratorKey,
									   Action execute, Func<bool> canExecute)
		{
			UICommand command = new(execute, canExecute) { AccessKey = accessKey, IconSource = icon, Label = label, };
			//command.KeyboardAccelerators.Add(CreateKeyboardAccelerator(modifier, acceleratorKey));

			return command;
		}

		static UICommand CreateCommand2(string label, string accessKey, Symbol symbol, VirtualKeyModifiers? modifier, VirtualKey acceleratorKey,
										Action execute, Func<bool> canExecute = null)
		{
			return CreateCommand(label, accessKey, new SymbolIconSource() { Symbol = symbol }, modifier, acceleratorKey, execute, canExecute);
		}

		static UICommand CreateCommand3(string label, string accessKey, char glyph, VirtualKeyModifiers? modifier, VirtualKey acceleratorKey,
										Action execute, Func<bool> canExecute = null)
		{
			return CreateCommand(label, accessKey, new FontIconSource() { Glyph = glyph.ToString() }, modifier, acceleratorKey, execute, canExecute);
		}

		//static KeyboardAccelerator CreateKeyboardAccelerator(VirtualKeyModifiers? modifier, VirtualKey acceleratorKey)
		//{
		//	KeyboardAccelerator keyboardAccelerator = new() { Key = acceleratorKey };

		//	if (modifier != null)
		//		keyboardAccelerator.Modifiers = modifier.Value;

		//	return keyboardAccelerator;
		//}

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


	private void ExecuteEditAction()
	{

	}

	#endregion

	#region Add new actions commands

	public readonly UICommand AddInsertActionCommand;
	public readonly UICommand AddInsertCounterActionCommand;
	public readonly UICommand AddRemoveActionCommand;
	public readonly UICommand AddReplaceActionCommand;
	public readonly UICommand AddConvertCaseActionCommand;


	private void ExecuteAddInsertAction()
	{
		Core.Actions.InsertAction item = new(new Core.Indices.BeginningIndexFinder(), "test #" + ViewModel.Project.Actions.Count);
		ViewModel.Project.Actions.Add(item);
		ViewModel.SelectedAction = item;
	}

	private void ExecuteAddInsertCounterAction()
	{
	}

	private void ExecuteAddRemoveAction()
	{
	}

	private void ExecuteAddReplaceAction()
	{
	}

	private void ExecuteAddConvertCaseAction()
	{
	}

	#endregion

	#region CanExecute predicates

	private bool CanExecuteWhenSelectedActionIsNotNull()
	{
		return ViewModel.SelectedAction != null;
	}

	private bool CanExecuteWhenSelectedActionIsNotFirst()
	{
		return ViewModel.SelectedAction != null && ViewModel.SelectedAction != ViewModel.Project.Actions[0];
	}

	private bool CanExecuteWhenSelectedActionIsNotLast()
	{
		return ViewModel.SelectedAction != null && ViewModel.SelectedAction != ViewModel.Project.Actions[^1];
	}

	private void UpdateCommandStates()
	{
		MoveUpActionCommand.NotifyCanExecuteChanged();
		MoveDownActionCommand.NotifyCanExecuteChanged();
		EditActionCommand.NotifyCanExecuteChanged();
		DuplicateActionCommand.NotifyCanExecuteChanged();
		RemoveActionCommand.NotifyCanExecuteChanged();
	}

	#endregion

	#endregion
}