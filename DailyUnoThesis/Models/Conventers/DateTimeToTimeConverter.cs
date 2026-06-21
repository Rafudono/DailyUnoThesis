using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;

public class DateTimeToTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dt)
        {
            if (dt.TimeOfDay == new TimeSpan(0, 0, 1))
                return string.Empty;
            return dt.ToString("HH:mm ");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
