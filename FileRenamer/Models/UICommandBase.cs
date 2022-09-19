using Windows.System;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;


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
	public string Label { get; }

	/// <summary>
	/// Gets or sets a description for this command.
	/// </summary>
	public string Description { get; }

	/// <summary>
	/// Gets the tooltip for this command.
	/// </summary>
	public string ToolTip
	{
		get
		{
			if (KeyboardAccelerator == null)
				return Description;

			System.Text.StringBuilder sb = new(Description);

			sb.Append("    (");

			if (KeyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Windows))
				sb.Append("Win + ");

			if (KeyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Control))
				sb.Append("Ctrl + ");

			if (KeyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Menu))
				sb.Append("Alt + ");

			if (KeyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Shift))
				sb.Append("Shift + ");

			sb.Append(KeyboardAccelerator.Key).Append(')');

			return sb.ToString();
		}
	}

	/// <summary>
	/// Gets or sets a glyph for this command.
	/// </summary>
	public IconSource IconSource { get; }

	/// <summary>
	/// Gets or sets the access key (mnemonic) for this command.
	/// </summary>
	public string AccessKey { get; }

	/// <summary>
	/// Gets the key combinations that invokes the action associated with this command.
	/// </summary>
	public KeyboardAccelerator KeyboardAccelerator { get; }


	/// <summary>
	/// Initializes a new instance of the <see cref="UICommandBase"/> class.
	/// </summary>
	/// <param name="description">The description for this command.</param>
	/// <param name="label">The label for this command.</param>
	/// <param name="accessKey">The access key (mnemonic) for this command.</param>
	/// <param name="modifier">The virtual key that must be pressed along with the <paramref name="acceleratorKey"/> to activate this command.</param>
	/// <param name="acceleratorKey">The key combinations that invokes the action associated with this command.</param>
	/// <param name="icon">The icon for this command.</param>
	public UICommandBase(string description, string label, string accessKey,
						 VirtualKeyModifiers? modifier, VirtualKey? acceleratorKey, IconSource icon)
	{
		Label = label;
		Description = description;
		IconSource = icon;
		AccessKey = accessKey;

		if (acceleratorKey != null)
			KeyboardAccelerator = CreateKeyboardAccelerator(modifier, acceleratorKey.Value);


		// Helper function

		static KeyboardAccelerator CreateKeyboardAccelerator(VirtualKeyModifiers? modifier, VirtualKey acceleratorKey)
		{
			KeyboardAccelerator keyboardAccelerator = new() { Key = acceleratorKey };

			if (modifier != null)
				keyboardAccelerator.Modifiers = modifier.Value;

			return keyboardAccelerator;
		}
	}


	/// <summary>
	/// Notifies that the <see cref="ICommand.CanExecute"/> property has changed.
	/// </summary>
	public abstract void NotifyCanExecuteChanged();
}