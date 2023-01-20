using System;
using System.Threading.Tasks;


namespace FileRenamer.Helpers;

public static class ActionDebounceExtension
{
	public static Func<Task> DebounceAsync(this Action action, int duration)
	{
		if (duration <= 0)
			throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

		DateTime lastCallTime = DateTime.MinValue;

		return debouncedAsync;


		async Task debouncedAsync()
		{
			lastCallTime = DateTime.UtcNow;

			await Task.Delay(duration);

			double elapsedTime = (DateTime.UtcNow - lastCallTime).TotalMilliseconds;

			if (duration <= elapsedTime)
				action();
		}
	}

	public static Action Debounce(this Action action, int duration)
	{
		if (duration <= 0)
			throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

		DateTime lastCallTime = DateTime.MinValue;

		return debounced;


		void debounced()
		{
			lastCallTime = DateTime.UtcNow;

			Task.Delay(duration).ContinueWith(core);

			void core(Task task)
			{ 
				double elapsedTime = (DateTime.UtcNow - lastCallTime).TotalMilliseconds;

				if (duration <= elapsedTime)
					action();
			}
		}
	}


	public static Action<T> Debounce<T>(this Action<T> action, int duration)
	{
		if (duration <= 0)
			throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

		DateTime lastCallTime = DateTime.MinValue;

		return debounced;


		void debounced(T args)
		{
			lastCallTime = DateTime.UtcNow;

			Task.Delay(duration).ContinueWith(core);

			void core(Task task)
			{ 
				double elapsedTime = (DateTime.UtcNow - lastCallTime).TotalMilliseconds;

				if (duration <= elapsedTime)
					action(args);
			}
		}
	}
}