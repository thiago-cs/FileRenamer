using System;
using System.ComponentModel;
using System.Reflection;


namespace FileRenamer.Converters;

internal sealed class EnumToDescriptionConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value is null ? null : value is Enum enumValue ? GetDescription(enumValue) : value.ToString();


		static string GetDescription(Enum value)
		{
			MemberInfo[] memberInfo = value.GetType().GetMember(value.ToString());

			return memberInfo.Length != 0 && memberInfo[0].GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attribute
				? attribute.Description
				: value.ToString();
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}