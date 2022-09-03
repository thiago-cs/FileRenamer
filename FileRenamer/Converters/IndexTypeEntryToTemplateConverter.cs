using System;
using Microsoft.UI.Xaml;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.Converters;

internal class IndexTypeEntryToTemplateConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	private static readonly DataTemplate emptyDataTemplate = new();

	public DataTemplate PositionTemplate { get; set; }
	public DataTemplate AfterBeforeTemplate { get; set; }


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value is IndexTypeEntry entry
			 ? GetDataTemplate(entry.Type)
			 : emptyDataTemplate;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}

	private DataTemplate GetDataTemplate(IndexType indexType)
	{
		return indexType switch
		{
			IndexType.Position => PositionTemplate,
			IndexType.After or IndexType.Before => AfterBeforeTemplate,
			IndexType.Beginning or IndexType.End or IndexType.FileExtension or _ => emptyDataTemplate,
		};
	}
}