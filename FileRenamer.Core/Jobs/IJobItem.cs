namespace FileRenamer.Core.Jobs;

public interface IJobItem
{
	void Run(JobTarget target, JobContext context);
}