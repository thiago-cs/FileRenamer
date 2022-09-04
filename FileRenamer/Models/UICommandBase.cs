namespace FileRenamer.Models;

public abstract class UICommandBase
{
	/// <summary>
	/// Gets the underlying <see cref="ICommand"/> object.
	/// </summary>
	public System.Windows.Input.ICommand Command { get; protected init; }

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

	/// <summary>
	/// Gets the key combinations that invokes the action associated with this command.
	/// </summary>
	public Microsoft.UI.Xaml.Input.KeyboardAccelerator KeyboardAccelerator { get; internal init; }


	/// <summary>
	/// Notifies that the <see cref="ICommand.CanExecute"/> property has changed.
	/// </summary>
	public abstract void NotifyCanExecuteChanged();
}