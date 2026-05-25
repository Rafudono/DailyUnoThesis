using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    public class HeightConventer : IValueConverter
    {
        // Превращает True в False, а False в True
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double b && b != 0)
            {
                return b;
            }
            return 100;
        }

        // Нужно для обратной привязки (если используется Mode=TwoWay)
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();

        }
    }
}
