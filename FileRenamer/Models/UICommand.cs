using System;


namespace FileRenamer.Models;

/// <summary>
/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
/// </summary>
/// <remarks>
/// The default return value for the CanExecute method is 'true'.<br/>
/// <see cref="NotifyCanExecuteChanged"/> needs to be called whenever <see cref="CanExecute"/> is expected to return a different value.
/// </remarks>
public sealed class UICommand : System.Windows.Input.ICommand
{
	#region Fields

	private readonly Action _execute;
	private readonly Func<bool> _canExecute;

	#endregion


	#region Propeties

	/// <summary>
	/// Gets or sets the label for this command.
	/// </summary>
	public string Label { get; set; }

	/// <summary>
	/// Gets or sets a description for this command.
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Gets or sets a glyph for this command.
	/// </summary>
	public Microsoft.UI.Xaml.Controls.IconSource IconSource { get; set; }

	/// <summary>
	/// Gets or sets the access key (mnemonic) for this command.
	/// </summary>
	public string AccessKey { get; set; }

	#endregion


	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="UICommand"/> class that can always be executed.
	/// </summary>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	public UICommand(Action execute)
		: this(execute, null)
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="UICommand"/> class.
	/// </summary>
	/// <param name="execute">The method to be called when the command is invoked.</param>
	/// <param name="canExecute">The method that determines whether the command can execute in its current state.</param>
	public UICommand(Action execute, Func<bool> canExecute)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute;
	}

	#endregion


	#region ICommand implementation

	public event EventHandler CanExecuteChanged;

	public bool CanExecute(object parameter)
	{
		return _canExecute == null || _canExecute();
	}

	public void Execute(object parameter)
	{
		_execute();
	}

	#endregion


	/// <summary>
	/// Notifies the system that the command state has changed.
	/// </summary>
	public void NotifyCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}