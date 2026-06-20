using DailyUnoThesis.Models.MainClasses;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace DailyUnoThesis.Models.Conventers;

internal class TagColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string? color = value switch
        {
            Tag tag => tag.Color,
            string s => s,
            _ => null
        };

        if (string.IsNullOrEmpty(color))
            return Application.Current.Resources["StickerBrush"];

        color = color.Trim();
        if (!color.StartsWith("#"))
            color = "#" + color;

        if (color.Length == 7 || color.Length == 9)
        {
            try
            {
                var hex = color.TrimStart('#');
                byte a = 255;
                int offset = 0;
                if (hex.Length == 8)
                {
                    a = Convert.ToByte(hex[..2], 16);
                    offset = 2;
                }
                byte r = Convert.ToByte(hex[offset..(offset + 2)], 16);
                byte g = Convert.ToByte(hex[(offset + 2)..(offset + 4)], 16);
                byte b = Convert.ToByte(hex[(offset + 4)..(offset + 6)], 16);
                return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
            catch { }
        }

        return Application.Current.Resources["StickerBrush"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
