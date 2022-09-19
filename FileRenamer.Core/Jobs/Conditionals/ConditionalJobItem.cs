namespace FileRenamer.Core.Jobs.Conditionals;


public abstract class ConditionalJobItem : ComplexJobItem
{
	protected ConditionalJobItem(JobCollection jobs)
		: base(jobs)
	{ }


	public abstract bool TestTarget(JobTarget target);
}