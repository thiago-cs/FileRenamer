using System.Collections.ObjectModel;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core;

/// <summary>
/// Represents a collection of actions to be executed in order on a string.
/// </summary>
public sealed class ActionCollection : ObservableCollection<RenameActionBase>
{
	/// <summary>
	/// Executes the actions in this collection, in order, on the specified input.
	/// </summary>
	/// <param name="input">A string.</param>
	/// <returns>The resulting string after the actions have been executed on it.</returns>
	public string Run(string input)
	{ 
		foreach (RenameActionBase action in this)
		{
			if (action.IsEnabled)
				input = action.Run(input);
		}

		return input;
	}
}