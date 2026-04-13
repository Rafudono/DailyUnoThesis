using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class DayOfWeekToShortStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DayOfWeek day)
        {
            var culture = new System.Globalization.CultureInfo("ru-RU");
            return culture.DateTimeFormat.GetShortestDayName(day);
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
