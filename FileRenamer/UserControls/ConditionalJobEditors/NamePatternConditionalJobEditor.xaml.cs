using FileRenamer.Core.Jobs.Conditionals;
using FileRenamer.UserControls.ActionEditors;


namespace FileRenamer.UserControls.ConditionalJobEditors;

public sealed partial class NamePatternConditionalJobEditor : IJobEditor<NamePatternConditionalJobData>
{
	public string DialogTitle => "If name...";

	public NamePatternConditionalJobData Data { get; }


	public NamePatternConditionalJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public NamePatternConditionalJobEditor(ItemNameJobConditional job)
	{
		Data = new(job);
		InitializeComponent();
	}
}