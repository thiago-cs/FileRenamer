using System;


namespace FileRenamer.Converters;

internal class CharToStringConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return new string((char)value, 1);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		return (value as string)[0];
	}
}