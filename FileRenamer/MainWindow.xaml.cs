﻿using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.ViewModels;


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

	public async Task<ContentDialogResult> ShowJobEditorDialogAsync(UserControls.ActionEditors.IActionEditor editor)
	{
		dialog.DataContext = editor;

		ContentDialogResult result = await dialog.ShowAsync();

		return result;
	}
}