using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (value is bool vis)
            {
                if (vis == true)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
