namespace FileRenamer.Core.Jobs;

public sealed class JobContext
{
	public JobCollection Jobs { get; }

	public ICollection<JobTarget> Targets { get; }

	private int _targetIndex = 0;
	public int TargetIndex
	{
		get => _targetIndex;

		private set
		{
			if (_targetIndex == value)
				return;

			_targetIndex = value;
			double progress = ((double)value) / Targets.Count;
			ProgressChanged?.Invoke(this, progress);
		}
	}

	/// <summary>
	/// value between 0 and 1.
	/// </summary>
	public event EventHandler<double>? ProgressChanged;


	public JobContext(JobCollection jobs, ICollection<JobTarget> targets)
	{
		Jobs = jobs;
		Targets = targets;
	}


	public void Next()
	{
		TargetIndex++;
	}
}