using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;


namespace FileRenamer.Models;

public class ExtendedUICommand : StandardUICommand
{
	/// <summary>
	/// Gets the tooltip for this command.
	/// </summary>
	public string ToolTip
	{
		get
		{
			if (KeyboardAccelerators.Count == 0)
				return Description;

			KeyboardAccelerator ka = KeyboardAccelerators[0];
			System.Text.StringBuilder sb = new(Description);

			sb.Append("    (");

			if (ka.Modifiers.HasFlag(VirtualKeyModifiers.Windows))
				sb.Append("Win + ");

			if (ka.Modifiers.HasFlag(VirtualKeyModifiers.Control))
				sb.Append("Ctrl + ");

			if (ka.Modifiers.HasFlag(VirtualKeyModifiers.Menu))
				sb.Append("Alt + ");

			if (ka.Modifiers.HasFlag(VirtualKeyModifiers.Shift))
				sb.Append("Shift + ");

			sb.Append(ka.Key).Append(')');

			return sb.ToString();
		}
	}

	public static SymbolIconSource CreateIconSource(Symbol symbol)
	{
		return new() { Symbol = symbol };
	}

	public static FontIconSource CreateIconSource(char glyph)
	{
		return new() { Glyph = glyph.ToString() };
	}
}