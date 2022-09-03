using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.UserControls.InputControls;
using FileRenamer.ViewModels.ValueSources;


namespace FileRenamer.Converters;

internal class ValueSourceTemplateSelector : DataTemplateSelector
{
	public DataTemplate StringTemplate { get; set; }
	public DataTemplate RandomStringTemplate { get; set; }
	public DataTemplate CounterTemplate { get; set; }


	protected override DataTemplate SelectTemplateCore(object item)
	{
		return item switch
		{
			StringValueSourceViewModel => StringTemplate,
			RandomStringValueSourceViewModel => RandomStringTemplate,
			CounterValueSourceViewModel => CounterTemplate,
			//Mp3Tag => throw new NotImplementedException(),
			//ExifTag => throw new NotImplementedException(),
			_ => null,
		};
	}

	protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
	{
		//if (container is FrameworkElement fe && fe.DataContext is InsertActionEditor editor)
		//	return editor.Data.ValueSourceType switch
		//	{
		//		ValueSourceType.FixedText => StringTemplate,
		//		ValueSourceType.RandomText => RandomStringTemplate,
		//		ValueSourceType.Counter => CounterTemplate,
		//		//StringSourceType.Mp3Tag => throw new NotImplementedException(),
		//		//StringSourceType.ExifTag => throw new NotImplementedException(),
		//		_ => null,
		//	};

		return SelectTemplateCore(item);
	}
}