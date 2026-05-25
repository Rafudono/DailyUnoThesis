using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
namespace DailyUnoThesis.Models.Conventers;
    public class MaxTimerConventer : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {
        if (value is int num)
        {
            // Формат "D2" превращает 5 в "05", а 12 оставляет как "12"
            if(num == 0)
                return null;
            return num.ToString("D2");
        }
        return null;
    }

    //public object ConvertBack(object value, Type targetType, object parameter, string language)
    //{
    //    throw new NotImplementedException();
    //}


    //public object ConvertBack(object value, Type targetType, object parameter, string language)
    //{
    //    try
    //    {

    //        var strVal = value as string;
    //        if (string.IsNullOrWhiteSpace(strVal))
    //            return "";

    //        int num = int.Parse(strVal);
    //        if (parameter != null && parameter.ToString() == "Limit59" && num > 60)
    //            return "60";
    //        else if (num < 100 && num >= 60)
    //            return "60"; // Ограничиваем до максимального значения

    //        else
    //            return num.ToString(language); // Возвращаем исходное значение
    //    }
    //    catch (Exception ex)
    //    {
    //        // Здесь можно обработать исключение, например, вернуть пустое значение
    //        Console.WriteLine($"Ошибка конвертации: {ex.Message}");
    //        return "";
    //    }
    //}

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        string strVal = value as string;

        if (string.IsNullOrWhiteSpace(strVal))
            return 0;

        // Оставляем только цифры (на случай, если попала буква)
        string cleanString = new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Where(strVal, char.IsDigit)));

        if (int.TryParse(cleanString, out int num))
        {
            // Если передан параметр "Limit59", ограничиваем число
            if (parameter?.ToString() == "Limit59")
            {
                if (num >= 60) return 59;
            }
            else if (parameter?.ToString() == "Classic")
            {
                if (num > 60) return 61;
                //if (num < 10) return 10;
            }
            else if (parameter?.ToString() == "TomatoAndRound")
            {
                if (num > 30) return 30;
                //if (num < 10) return 10;
            }
            else if (num > 1) return 1;

            return num; // Возвращаем int напрямую во ViewModel
        }

        return 0;
    }

}
