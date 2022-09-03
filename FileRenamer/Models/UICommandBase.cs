using System;
using System.Windows.Input;


namespace FileRenamer.Models;

public abstract class UICommandBase
{
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
	/// Notifies the system that the command state has changed.
	/// </summary>
	public abstract void NotifyCanExecuteChanged();
}