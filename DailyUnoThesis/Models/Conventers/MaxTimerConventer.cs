using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
namespace DailyUnoThesis.Models.Conventers
{
    public class MaxTimerConventer : IValueConverter
    {

         
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {

                var strVal = value as string;
                if (string.IsNullOrWhiteSpace(strVal))
                    return "";

                int num = int.Parse(strVal);
                if (parameter != null && parameter.ToString() == "OnlyMaxLeght" && num > 60)
                    return "59";
                else if (num < 100 && num > 60)
                    return "59"; // Ограничиваем до максимального значения

                else
                    return num.ToString(language); // Возвращаем исходное значение
            }
            catch (Exception ex)
            {
                // Здесь можно обработать исключение, например, вернуть пустое значение
                Console.WriteLine($"Ошибка конвертации: {ex.Message}");
                return "";
            }
        }
    }
}
