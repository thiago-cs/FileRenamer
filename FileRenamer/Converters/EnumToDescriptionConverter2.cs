using FileRenamer.Core.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;


namespace FileRenamer.Converters;

public class DescriptionMap
{
	private readonly Dictionary<string, Enum> _values = new();
	private readonly Dictionary<Enum, string> _descriptions = new();


	public Type Type { get; }

	public Enum this[string description]
	{
		get => _values[description];
		set => _values[description] = value;
	}

	public string this[Enum @enum]
	{
		get => _descriptions[@enum];
		set => _descriptions[@enum] = value;
	}


	public DescriptionMap(Type type)
	{
		Type = type;
		Array values = Enum.GetValues(type);

		foreach (Enum value in values)
		{
			string description = GetDescription(value);

			_descriptions[value] = description;
			_values[description] = value;
		}

		string GetDescription(Enum value)
		{
			MemberInfo[] memberInfo = type.GetMember(value.ToString());

			return memberInfo.Length != 0 && memberInfo[0].GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attribute
				? attribute.Description
				: value.ToString();
		}
	}
}

internal sealed class EnumToDescriptionConverter2 : Microsoft.UI.Xaml.Data.IValueConverter
{
	private DescriptionMap map;

	public Type Type
	{
		get => map.Type;
		set => map = new DescriptionMap(value);
	}


	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value switch { Enum enumValue => map[enumValue], null => null, _ => value.ToString() };
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		return value is string description ? map[description] : null;
	}
}

internal sealed class IntToScopeConverter : Microsoft.UI.Xaml.Data.IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return (JobScope)value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		return (int)value;
	}
}