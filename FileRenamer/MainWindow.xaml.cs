﻿using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.ViewModels;
using FileRenamer.UserControls.ActionEditors;


namespace FileRenamer;

public sealed partial class MainWindow
{
	internal MainWindowViewModel ViewModel { get; }


	public MainWindow()
	{
		ViewModel = new(this);

		InitializeComponent();
		//ExtendsContentIntoTitleBar = true;
	}


	public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
	{
		Panel panel = Content as Panel;
		panel.Children.Add(dialog);

		ContentDialogResult result = await dialog.ShowAsync();

		panel.Children.Remove(dialog);
		return result;
	}

	public async Task<ContentDialogResult> ShowJobEditorDialogAsync<T>(IJobEditor<T> editor) where T : IJobEditorData
	{
		dialog.DataContext = editor;

		ContentDialogResult result = await dialog.ShowAsync();

		return result;
	}

	private void FolderView_TestRequested(object sender, EventArgs e)
	{
		var itemNode = sender as Core.Jobs.JobTarget;
		MyTextPreviewer.ProcessName(itemNode.StorageItem.Name);
	}

	// Look what you made me do!
	private UserControls.TextPreviewer MyTextPreviewer;
	private void MyTextPreviewer_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		MyTextPreviewer = sender as UserControls.TextPreviewer;
	}
}