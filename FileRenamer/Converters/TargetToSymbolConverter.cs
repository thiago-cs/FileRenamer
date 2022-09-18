using System;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Core.FileSystem;
using FileRenamer.Core.Jobs;


namespace FileRenamer.Converters;

internal class TargetToSymbolConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public Symbol FileSymbol { get; set; }
	public Symbol FolderSymbol { get; set; }


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value is not JobTarget target ? null
			 : target.StorageItem is IFolder ? FolderSymbol
			 : FileSymbol;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}