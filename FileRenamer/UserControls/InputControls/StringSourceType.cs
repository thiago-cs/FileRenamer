using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public enum StringSourceType
{
	[Description("text")]
	Text,

	[Description("a counter")]
	Counter,

	//[Description("MP3 tag")]
	//Mp3Tag,

	//[Description("EXIF tag")]
	//ExifTag,
}