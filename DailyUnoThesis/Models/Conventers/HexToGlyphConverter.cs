using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;

public class HexToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string hex && !string.IsNullOrEmpty(hex))
        {
            // Конвертируем строку "E814" в символ Юникода
            if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int codePoint))
            {
                return char.ConvertFromUtf32(codePoint);
            }
        }

        // Если иконка не задана, возвращаем стандартную папку 📁 (E8B7)
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
}

