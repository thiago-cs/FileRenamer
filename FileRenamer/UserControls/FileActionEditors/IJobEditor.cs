namespace FileRenamer.UserControls.ActionEditors;

public interface IJobEditor<out T> where T : IJobEditorData
{
	public string DialogTitle { get; }
	public T Data { get; }
}