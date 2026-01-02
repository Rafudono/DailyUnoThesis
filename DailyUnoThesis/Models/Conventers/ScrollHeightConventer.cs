using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    class ScrollHeightConventer : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double height)
            {
                return height;
            }
            return 00;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
