using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;


namespace FileRenamer.Models;

public sealed class UICommand : UICommandBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SyncUICommand"/> class.
	/// </summary>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public UICommand(Action execute, Func<bool> canExecute = null)
	{
		Command = canExecute == null
					? new RelayCommand(execute)
					: new RelayCommand(execute, canExecute);
	}


	public override void NotifyCanExecuteChanged()
	{
		(Command as RelayCommand).NotifyCanExecuteChanged();
	}
}

public sealed class AsyncUICommand : UICommandBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AsyncUICommand"/> class.
	/// </summary>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public AsyncUICommand(Func<Task> execute, Func<bool> canExecute = null)
	{
		Command = canExecute == null
					? new AsyncRelayCommand(execute)
					: new AsyncRelayCommand(execute, canExecute);
	}


	public override void NotifyCanExecuteChanged()
	{
		(Command as AsyncRelayCommand).NotifyCanExecuteChanged();
	}
}