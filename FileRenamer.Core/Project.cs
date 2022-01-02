using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core;

public sealed class Project : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
{
	public readonly ActionCollection _actions = new();

	private string? _folderPath;
	public string? FolderPath { get => _folderPath; set => SetProperty(ref _folderPath, value); }


	public async Task Run()
	{
		if (_folderPath == null)
			return;

		await Task.CompletedTask;
	}
}