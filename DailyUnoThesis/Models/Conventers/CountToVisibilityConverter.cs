using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Если это коллекция и в ней есть элементы — показываем стрелку
            if (value is int count) return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            if (value is IEnumerable list) return list.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
