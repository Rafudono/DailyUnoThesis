using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class LevelToMarginConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        int level = (int)value;
        // Воспроизводим вашу логику из WPF:
        if (level == 0) return new Thickness(60, 0, 0, 0);
        if (level == 2) return new Thickness(25, 0, 0, 0);

        return new Thickness(0); // По умолчанию
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
