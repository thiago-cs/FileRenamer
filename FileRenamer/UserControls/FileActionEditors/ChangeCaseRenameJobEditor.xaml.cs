using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class ChangeCaseRenameJobEditor : UserControl, IJobEditor
{
	#region Properties

	public string DialogTitle => "Change case";

	public ChangeCaseRenameJobData Data { get; }
	IJobEditorData IJobEditor.Data => Data;

	#endregion


	#region Contructors

	public ChangeCaseRenameJobEditor()
	{
		Data = new();
		InitializeComponent();
	}

	public ChangeCaseRenameJobEditor(ChangeRangeCaseAction action)
	{
		Data = new(action);
		InitializeComponent();
	}

	public ChangeCaseRenameJobEditor(ChangeStringCaseAction action)
	{
		Data = new(action);
		InitializeComponent();
	}

	#endregion

}