using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class BoolToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // Если раскрыто — шляпка вниз, если закрыто — шляпка вправо
        return (bool)value ? "M 0,0 L 20,0 L 10,10 Z" : "M 0,0 L 10,10 L 0,20 Z";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

