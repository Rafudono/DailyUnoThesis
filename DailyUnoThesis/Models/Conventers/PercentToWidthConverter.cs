using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class PercentToWidthConverter : IValueConverter
{
    // value - это процент (например, 10.5)
    // parameter - это полная ширина контейнера (например, 800)
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // Превращаем значение и параметр в числа
        if (value != null && double.TryParse(value.ToString(), out double percent))
        {
            double totalWidth = 800; // Значение по умолчанию
            if (parameter != null && double.TryParse(parameter.ToString(), out double parsedWidth))
            {
                totalWidth = parsedWidth;
            }

            return (percent / 100.0) * totalWidth;
        }
        return 0.0;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();  
    }
}
