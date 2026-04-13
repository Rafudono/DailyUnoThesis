using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Text;

namespace DailyUnoThesis.Models.Conventers;

public class BoolToFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isToday)
        {
            return isToday ? FontWeights.Bold : FontWeights.Normal;
        }
        return FontWeights.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
