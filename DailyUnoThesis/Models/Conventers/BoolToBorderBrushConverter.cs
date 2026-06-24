using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DailyUnoThesis.Models.Conventers;

internal class BoolToBorderBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var defaultBrush = new SolidColorBrush(Color.FromArgb(255, 203, 179, 156));
        var alertBrush = new SolidColorBrush(Color.FromArgb(255, 101, 22, 32));

        if (value is bool hasInvitations)
            return hasInvitations ? alertBrush : defaultBrush;

        return defaultBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
