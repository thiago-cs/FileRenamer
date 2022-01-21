using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation.Collections;


namespace System.Collections.Generic;

public class ObservableVector<T> : Collection<T>, IObservableVector<T>, INotifyPropertyChanged
{
	#region Nested types

	/// <summary>
	/// Provides the default implementation of the <see cref="IVectorChangedEventArgs"/> interface.
	/// </summary>
	protected class VectorChangedEventArgs : IVectorChangedEventArgs
	{
		public CollectionChange CollectionChange { get; }

		public uint Index { get; }


		/// <summary>
		/// Innitializes a new instance of the <see cref="VectorChangedEventArgs"/>.
		/// </summary>
		/// <param name="change">The type of change that occurred in the vector.</param>
		/// <param name="index">The position where the change occurred in the vector.</param>
		public VectorChangedEventArgs(CollectionChange change, uint index)
		{
			Index = index;
			CollectionChange = change;
		}
	}

	#endregion


	#region Fields

	private const string IndexerName = "Item[]";

	// Cached EventArgs

	private static readonly VectorChangedEventArgs ResetArgs = new(CollectionChange.Reset, 0);
	private static readonly PropertyChangedEventArgs IndexerArgs = new(IndexerName);
	private static readonly PropertyChangedEventArgs ItemsArgs = new(nameof(Items));
	private static readonly PropertyChangedEventArgs CountArgs = new(nameof(Count));

	#endregion


	#region Events

	public event PropertyChangedEventHandler PropertyChanged;
	public event VectorChangedEventHandler<T> VectorChanged;

	#endregion


	#region Constructors

	public ObservableVector()
	{ }

	public ObservableVector(IList<T> list)
		: base(list)
	{ }

	#endregion


	#region Overrides

	protected override void ClearItems()
	{
		base.ClearItems();
		OnCountChanged();
		OnItemsChanged();
		OnIndexerChanged();
		OnVectorChanged(ResetArgs);
	}

	protected override void InsertItem(int index, T item)
	{
		base.InsertItem(index, item);
		OnCountChanged();
		OnItemsChanged();
		OnIndexerChanged();
		OnVectorChanged(CollectionChange.ItemInserted, (uint)index);
	}

	protected override void RemoveItem(int index)
	{
		base.RemoveItem(index);
		OnCountChanged();
		OnItemsChanged();
		OnIndexerChanged();
		OnVectorChanged(CollectionChange.ItemRemoved, (uint)index);
	}

	protected override void SetItem(int index, T item)
	{
		base.SetItem(index, item);
		OnItemsChanged();
		OnIndexerChanged();
		OnVectorChanged(CollectionChange.ItemChanged, (uint)index);
	}

	#endregion


	#region Event wrapers

	// Protected event wrapper methods

	protected void OnVectorChanged(VectorChangedEventArgs e) => VectorChanged?.Invoke(this, e);

	protected void OnVectorChanged(CollectionChange change, uint index) => OnVectorChanged(new VectorChangedEventArgs(change, index));

	protected void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

	protected void OnPropertyChanged(string name) => OnPropertyChanged(new PropertyChangedEventArgs(name));

	// Event wrapper-wrapper methods!

	private void OnItemsChanged() => OnPropertyChanged(ItemsArgs);

	private void OnCountChanged() => OnPropertyChanged(CountArgs);

	private void OnIndexerChanged() => OnPropertyChanged(IndexerArgs);

	#endregion
}