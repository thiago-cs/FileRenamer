using FileRenamer.Core.Jobs.Conditionals;
using FileRenamer.UserControls.ActionEditors;


namespace FileRenamer.UserControls.ConditionalJobEditors;

public sealed partial class NamePatternConditionalJobEditor : IJobEditor
{
	public string DialogTitle => "If name...";

	public NamePatternConditionalJobData Data { get; }
	IJobEditorData IJobEditor.Data => Data;


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