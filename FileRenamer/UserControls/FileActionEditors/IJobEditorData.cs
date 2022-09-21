using System.ComponentModel;
using FileRenamer.Core.Jobs;


namespace FileRenamer.UserControls.ActionEditors;

public interface IJobEditorData : INotifyDataErrorInfo
{
	public JobItem GetJobItem();
}