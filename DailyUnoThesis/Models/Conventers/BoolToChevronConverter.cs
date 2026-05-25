using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    public class BoolToChevronConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "▲" : "▼"; 
            }
            return "▼"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
