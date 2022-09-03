﻿using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.Mvvm.Input;


namespace FileRenamer.Models;

public sealed class UICommand : UICommandBase
{
	public RelayCommand Command { get; }
	public KeyboardAccelerator KeyboardAccelerator { get; internal init; }


	/// <summary>
	/// Initializes a new instance of the <see cref="SyncUICommand"/> class.
	/// </summary>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public UICommand(Action execute, Func<bool> canExecute = null)
	{
		Command = canExecute == null
					? new(execute)
					: new(execute, canExecute);
	}


	public override void NotifyCanExecuteChanged()
	{
		Command.NotifyCanExecuteChanged();
	}
}

public sealed class AsyncUICommand : UICommandBase
{
	public AsyncRelayCommand Command { get; }


	/// <summary>
	/// Initializes a new instance of the <see cref="AsyncUICommand"/> class.
	/// </summary>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public AsyncUICommand(Func<Task> execute, Func<bool> canExecute = null)
	{
		Command = canExecute == null
					? new(execute)
					: new(execute, canExecute);
	}


	public override void NotifyCanExecuteChanged()
	{
		Command.NotifyCanExecuteChanged();
	}
}