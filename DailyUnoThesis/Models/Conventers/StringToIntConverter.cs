using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
          


            if (value is int integer)
                return integer.ToString();
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty((string)value))
                return null;

            try
            {
                return int.Parse((string)value);
            }
            catch (FormatException e)
            {
                throw new FormatException("Invalid input format.", e);
            }
        }
    }
}
