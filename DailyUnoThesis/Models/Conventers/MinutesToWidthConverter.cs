using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class MinutesToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // 'value' — это DurationMinutes из API (double)
        if (value is double minutes)
        {
            // 'parameter' — это коэффициент масштаба (сколько пикселей в одной минуте)
            // Если параметр не передан в XAML, берем 2.0 по умолчанию
            double scale = 2.0;

            if (parameter != null && double.TryParse(parameter.ToString(), out double customScale))
            {
                scale = customScale;
            }

            return minutes * scale;
        }

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException(); // Не используется
    }
}
