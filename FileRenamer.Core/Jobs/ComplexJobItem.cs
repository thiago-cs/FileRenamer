namespace FileRenamer.Core.Jobs;

public abstract class ComplexJobItem : JobItem
{
    public JobCollection Jobs { get; }


    protected ComplexJobItem(JobCollection jobs)
    {
        Jobs = jobs;
    }
}