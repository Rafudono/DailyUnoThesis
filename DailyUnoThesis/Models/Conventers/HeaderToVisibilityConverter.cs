using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Models.MainClasses;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;
public class HeaderToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null) return null;
        //if (value is List<Category> categories)
        //{
            //if (value.Count < 1)
            //{
            //    if (value[0].IdUpCategory != 0 && value[0].IdUpCategory != null)
            //        return Visibility.Visible;
            //    else
            //        return Visibility.Collapsed;
            //}
            //return Visibility.Visible;
        //}


        if (value is ICollection categories) // ICollection - это базовый интерфейс для List, ObservableCollection и т.д.
        {
            if (categories.Count == 1)
            {
                var firstItem = categories.Cast<object>().FirstOrDefault();

                if (firstItem != null && firstItem is Category category) // Теперь безопасно кастим к вашему DTO
                {
                    // 4. Ваша логика:
                    // Если у первого элемента IdUpCategory равен 0 или null, значит, это корневой элемент,
                    // и мы хотим что-то скрыть/показать.
                    // Если вы хотите проверять IdUpCategory != 0 ИЛИ != null
                    // У CategoryDto IdUpCategory должен быть nullable int (int?)

                    // Предполагаем, что ваш CategoryDto имеет свойство IdUpCategory (int?)
                    if (category.IdUpCategory != null && category.IdUpCategory != 0)
                    {
                        // Например, хотим показать, если это НЕ корневая категория
                        return Visibility.Visible;
                    }
                    else
                    {
                        // Например, хотим скрыть, если это корневая категория или у нее нет родителя
                        return Visibility.Collapsed;
                    }
                }
            }
        }
       
        // Если value не коллекция или null, или что-то пошло не так, скрываем по умолчанию
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
