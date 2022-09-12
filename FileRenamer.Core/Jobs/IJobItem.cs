using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs;

public interface IJobItem: IXmlSerializableAsync
{
	void Run(JobTarget target, JobContext context);
}