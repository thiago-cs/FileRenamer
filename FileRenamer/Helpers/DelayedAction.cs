using System;
using System.Threading.Tasks;


namespace FileRenamer.Helpers;

/// <summary>
/// Encapsulates a method that has no parameters and does not return a value, and provides a way to execute that method with delay.
/// </summary>
public sealed class DelayedAction
{
	private readonly Action action;
	private DateTime timestamp;


	/// <summary>
	/// Creates a new instance of the <see cref="DelayedAction"/> class.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <exception cref="ArgumentNullException">The action argument is null.</exception>
	public DelayedAction(Action action)
	{
		this.action = action ?? throw new ArgumentNullException(nameof(action));
	}


	/// <summary>
	/// Schedules the execution of the app to after the specified delay.
	/// </summary>
	/// <param name="delay">The number of milliseconds to wait before executing the action.</param>
	/// <returns>A task that completes after the specified delay, irrespective of whether the action was executed.</returns>
	public async Task InvokeAsync(int delay)
	{
		timestamp = DateTime.UtcNow;

		await Task.Delay(delay);

		double elapsed = (DateTime.UtcNow - timestamp).TotalMilliseconds;

		if (delay < elapsed)
			action();
	}
}