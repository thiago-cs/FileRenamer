using System;
using Microsoft.UI.Xaml;


namespace FileRenamer.Converters;

internal class NullToVisibilityConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public Visibility IfNull { get; set; } = Visibility.Collapsed;
	public Visibility IfNotNull { get; set; } = Visibility.Visible;


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value == null ? IfNull : IfNotNull;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}