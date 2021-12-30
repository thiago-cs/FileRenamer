using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Core.Actions;


namespace FileRenamer.Core;

public sealed class Project : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, IList<RenameActionBase>
{
	private readonly List<RenameActionBase> _actions;


	private string _folderPath;
	public string FolderPath { get => _folderPath; set => SetProperty(ref _folderPath, value); }


	public async Task Run()
	{
		await Task.CompletedTask;
	}


	#region IList interface implementation

	public int Count => _actions.Count;

	public bool IsReadOnly => false;

	public RenameActionBase this[int index] { get => _actions[index]; set => _actions[index] = value; }

	public void Add(RenameActionBase item) => _actions.Add(item);

	public void Insert(int index, RenameActionBase item) => _actions.Insert(index, item);

	public bool Remove(RenameActionBase item) => _actions.Remove(item);

	public void RemoveAt(int index) => _actions.RemoveAt(index);

	public void Clear() => _actions.Clear();

	public int IndexOf(RenameActionBase item) => _actions.IndexOf(item);

	public bool Contains(RenameActionBase item) => _actions.Contains(item);

	public void CopyTo(RenameActionBase[] array, int arrayIndex) => _actions.CopyTo(array, arrayIndex);

	public IEnumerator<RenameActionBase> GetEnumerator() => _actions.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => _actions.GetEnumerator();

	#endregion
}