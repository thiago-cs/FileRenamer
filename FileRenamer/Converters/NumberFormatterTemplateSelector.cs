using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.Converters;

internal class NumberFormatterTemplateSelector : DataTemplateSelector
{
	public DataTemplate NumberToWordsFormatter { get; set; }
	public DataTemplate PaddedNumberFormatter { get; set; }
	public DataTemplate RomanNumberFormatter { get; set; }


	protected override DataTemplate SelectTemplateCore(object item)
	{
		return item is NumberToWordsFormatter ? NumberToWordsFormatter
			 : item is PaddedNumberFormatter ? PaddedNumberFormatter
			 : item is RomanNumberFormatter ? RomanNumberFormatter
			 : null;
	}
	protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
	{
		return SelectTemplateCore(item);
	}
}