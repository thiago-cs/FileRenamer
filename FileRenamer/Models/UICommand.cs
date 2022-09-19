using System;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;


namespace FileRenamer.Models;

public sealed class UICommand : UICommandBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SyncUICommand"/> class.
	/// </summary>
	/// <param name="description">The description for this command.</param>
	/// <param name="label">The label for this command.</param>
	/// <param name="accessKey">The access key (mnemonic) for this command.</param>
	/// <param name="modifier">The virtual key that must be pressed along with the <paramref name="acceleratorKey"/> to activate this command.</param>
	/// <param name="acceleratorKey">The key combinations that invokes the action associated with this command.</param>
	/// <param name="icon">The icon for this command.</param>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public UICommand(string description, string label, string accessKey,
					 VirtualKeyModifiers? modifier, VirtualKey? acceleratorKey, IconSource icon,
					 Action execute, Func<bool> canExecute = null)
		: base(description, label, accessKey, modifier, acceleratorKey, icon)
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
	/// <param name="description">The description for this command.</param>
	/// <param name="label">The label for this command.</param>
	/// <param name="accessKey">The access key (mnemonic) for this command.</param>
	/// <param name="modifier">The virtual key that must be pressed along with the <paramref name="acceleratorKey"/> to activate this command.</param>
	/// <param name="acceleratorKey">The key combinations that invokes the action associated with this command.</param>
	/// <param name="icon">The icon for this command.</param>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public AsyncUICommand(string description, string label, string accessKey,
						  VirtualKeyModifiers? modifier, VirtualKey? acceleratorKey, IconSource icon,
						  Func<Task> execute, Func<bool> canExecute = null)
		: base(description, label, accessKey, modifier, acceleratorKey, icon)
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