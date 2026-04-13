using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null) return null;
        if (parameter == null) return value;
        if (value is DateTime date)
        {
            DateTime stringDate = date;
            if (date.Date == DateTimeOffset.Now.Date)
            {
                return "Сегодня";
            }
            else if (date.Date == DateTimeOffset.Now.AddDays(1).Date)
            {
                return "Завтра";
            }
            else
            {
                if (date.Year == DateTimeOffset.Now.Year)
                    return stringDate.ToString("d MMM");
                else
                    return stringDate.ToString("d MMM yyyy");
            }
        }

        return string.Format((string)parameter, value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();

}
