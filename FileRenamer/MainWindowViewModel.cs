using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Core;


namespace FileRenamer;

public sealed class MainWindowViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject
{
	private Project _project;
	public Project Project { get => _project; set => SetProperty(ref _project, value); }
}