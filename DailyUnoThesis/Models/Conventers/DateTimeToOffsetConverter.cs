using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class DateTimeToOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dt)
        {
            // Обработка минимального значения (если дата не установлена)
            if (dt == DateTime.MinValue) return null;

            return new DateTimeOffset(dt);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTimeOffset dto)
        {
            return dto.DateTime;
        }

        // Если пользователь очистил дату, возвращаем MinValue или текущую дату
        return DateTime.MinValue;
    }
}
