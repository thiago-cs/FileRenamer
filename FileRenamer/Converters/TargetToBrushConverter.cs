using System;
using Microsoft.UI.Xaml.Media;
using FileRenamer.Core.Jobs;


namespace FileRenamer.Converters;

internal class TargetToBrushConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public Brush MovedItemBrush { get; set; }
	public Brush RenamedItemBrush { get; set; }
	public Brush MovedAndRenamedItemBrush { get; set; }
	public Brush DeletedItemBrush { get; set; }
	public Brush DefaultFileBrush { get; set; }
	public Brush DefaultFolderBrush { get; set; }


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is not JobTarget item)
			return null;

		bool wasMoved = item.NewFolderName != item.FolderName;
		bool wasRenamed = item.NewFileName != item.FileName;

		Brush brush = wasMoved && wasRenamed ? MovedAndRenamedItemBrush
					: wasMoved ? MovedItemBrush
					: wasRenamed ? RenamedItemBrush
					: IsDeleted(item) ? DeletedItemBrush
					: null;

		return brush ?? (item.StorageItem is Core.FileSystem.IFile ? DefaultFileBrush : DefaultFolderBrush);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}


	private static bool IsDeleted(JobTarget item) => item.NewFileName == null; // I am still not sold on this idea.
}