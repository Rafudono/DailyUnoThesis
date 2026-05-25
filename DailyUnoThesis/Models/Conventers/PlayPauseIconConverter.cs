using System;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Models.DobleClasses;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    class PlayPauseIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Предполагаем, что value — это bool (IsRunning)
            //var vm = value as СountdownTimer;
            bool isIcon = (value is bool) && (bool)value;
            //bool isTimming = (parameter is bool) && (bool) parameter;
         
                // Если запущен — возвращаем иконку Паузы, если нет — Запуска
                return (isIcon) ? "\uE768" : "\uE769";
            //}
            //else return "\uE768";
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
