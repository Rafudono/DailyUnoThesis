using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;
using Windows.ApplicationModel.DataTransfer;

namespace DailyUnoThesis.Models.Conventers;
internal class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isOtherMonth)
        {
            if (parameter is string colorName)
            {
                return colorName switch
                {
                    "LightGray" => isOtherMonth
                        ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 211, 211, 211))
                        : new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0)),
                    "Gray" => isOtherMonth
                        ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128))
                        : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)),
                    // ДОБАВЬТЕ ЭТОТ КЕЙС
                    "LightBlue" => isOtherMonth
                        ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 211, 211, 211))
                        : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 230, 245, 255)), // очень бледный голубой
                    _ => new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
                };
            }
            return new SolidColorBrush(isOtherMonth
                ? Windows.UI.Color.FromArgb(255, 211, 211, 211)
                : Windows.UI.Color.FromArgb(0, 0, 0, 0));
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
        //if (value is bool isOtherMonth)
        //{
        //    if (parameter is string colorName)
        //    {
        //        return colorName switch
        //        {
        //            "LightGray" => isOtherMonth
        //                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 211, 211, 211)) // LightGray
        //                : new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0)),
        //            "Gray" => isOtherMonth
        //                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128)) // Gray
        //                : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)), // Black
        //            _ => new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0))
        //        };
        //    }
        //    return new SolidColorBrush(isOtherMonth
        //        ? Windows.UI.Color.FromArgb(255, 211, 211, 211)
        //        : Windows.UI.Color.FromArgb(0, 0, 0, 0));
        //}
        //return new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}


