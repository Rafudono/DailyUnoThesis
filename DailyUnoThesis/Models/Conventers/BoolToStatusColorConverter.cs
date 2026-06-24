using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DailyUnoThesis.Models.Conventers;

internal class BoolToStatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var successBrush = new SolidColorBrush(Color.FromArgb(255, 76, 175, 80));
        var errorBrush = new SolidColorBrush(Color.FromArgb(255, 163, 49, 49));

        if (value is bool isSuccess)
            return isSuccess ? successBrush : errorBrush;

        return errorBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
