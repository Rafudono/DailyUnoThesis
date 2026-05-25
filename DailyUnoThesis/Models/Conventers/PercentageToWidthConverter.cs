using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class PercentageToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double percentage && parameter is string maxWidthString && double.TryParse(maxWidthString, out double maxWidth))
        {
            return (percentage / 100.0) * maxWidth;
        }
        return 0; // Возвращаем 0, если что-то пошло не так
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException(); // Не используется
    }
}
