using System;


namespace FileRenamer.Converters;

internal class InvertedBooleanConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return !(bool)value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		return !(bool)value;
	}
}