using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
internal class ArrowControlEnabledConventer : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null) return null;

        if (value is int level)
        {

            if (level >= 60 && parameter?.ToString() == "Classic")
            {
                return false;
            }
            if (level >= 15 && parameter?.ToString() == "Short")
            {
                return false;
            }
            if (level >= 60 && parameter?.ToString() == "Big")
            {
                return false;
            }
            if (level >= 10 && parameter?.ToString() == "Count")
            {
                return false;
            }
            else
            {
                return true;
            }

           
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
