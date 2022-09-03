using System.ComponentModel;
using FileRenamer.Core.ValueSources;


namespace FileRenamer.ViewModels.ValueSources;

public interface IValueSourceViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
	public IValueSource ValueSource { get; }
}