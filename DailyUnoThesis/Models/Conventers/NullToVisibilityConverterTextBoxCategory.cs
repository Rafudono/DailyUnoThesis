using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
internal class NullToVisibilityConverterTextBoxCategory : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        //return (value == null || (value is int i && i == 0))
        //       ? Visibility.Collapsed
        //       : Visibility.Visible;
        if (value == null || (value is int i && i == 0))
        {
            return Visibility.Collapsed;
        }
        else if(value == null)
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
