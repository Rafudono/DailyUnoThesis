using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers
{
    public class RootToMarginConverter : IValueConverter
    {
        // Отступ для главной ветви (сверху 24px)
        public Thickness RootMargin { get; set; } = new Thickness(0, 50, 0, 0);

        // Отступ для подзадач (сверху 4px)
        public Thickness ChildMargin { get; set; } = new Thickness(0, 4, 0, 0);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Проверяем, является ли задача корневой. 
            // Это может быть bool (IsRoot) или проверка на наличие родителя (ParentTaskId == null)
            //if (value is bool isRoot)
            //{
            //    return isRoot ? RootMargin : ChildMargin;
            //}

            // Если проверяем по ID родителя (если null - значит это корень)
            if (value == null || (value is int isRoot && isRoot == 0))
            {
                return RootMargin;
            }

            return ChildMargin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

}
