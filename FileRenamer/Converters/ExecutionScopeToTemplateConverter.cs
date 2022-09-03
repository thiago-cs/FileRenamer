using System;
using Microsoft.UI.Xaml;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.Converters;

internal class ExecutionScopeToTemplateConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	private static readonly DataTemplate emptyDataTemplate = new();

	public DataTemplate RangeTemplate { get; set; }
	public DataTemplate OccurrencesTemplate { get; set; }


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		DataTemplate template = value switch
		{
			ExecutionScope.CustomRange => RangeTemplate,
			ExecutionScope.Occurrences => OccurrencesTemplate,
			_ => null,
		};

		return template ?? emptyDataTemplate;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}