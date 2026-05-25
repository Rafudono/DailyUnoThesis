using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
   public  class StringFormatTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "00:00:00";

            // Если пришел TimeSpan (а в RemainingTime обычно он)
            if (value is TimeSpan timeSpan)
            {
                string format = parameter as string;
                //if (string.IsNullOrEmpty(format))
                format = @"hh\:mm\:ss"; // дефолтный формат, если параметр забыли

                return timeSpan.ToString(format);
            }
            else if (value is TimeOnly timeOnly)
            {
                string format = parameter as string;
                //if (string.IsNullOrEmpty(format))
                format = @"HH\:mm\:ss"; // дефолтный формат, если параметр забыли

                return timeOnly.ToString(format);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
