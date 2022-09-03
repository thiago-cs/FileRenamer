using System;
using Microsoft.UI.Xaml;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.Converters;

internal class TextRemovalTypeToTemplateConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	private static readonly DataTemplate emptyDataTemplate = new();

	public DataTemplate CountTemplate { get; set; }
	public DataTemplate EndIndexTemplate { get; set; }


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value is TextRangeType type
			 ? GetDataTemplate(type)
			 : emptyDataTemplate;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}

	private DataTemplate GetDataTemplate(TextRangeType type)
	{
		return type switch
		{
			TextRangeType.Count => CountTemplate,
			TextRangeType.Range => EndIndexTemplate,
			_ => emptyDataTemplate,
		};
	}
}