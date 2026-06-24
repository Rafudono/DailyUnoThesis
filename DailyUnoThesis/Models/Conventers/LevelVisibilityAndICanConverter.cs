using System;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Models.MainClasses;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;

public class LevelVisibilityAndICanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null) return null;
       
        if (value is Mission mis)
        {
            if (parameter is string id)
            {
                if (id == "id")
                {
                    if (mis.Id != 0 || (mis.UserId != AuthorizedUser.GetInstance().AuthUser.Id))
                        return Visibility.Collapsed;
                }
            }
            if (mis.LevelUp == 0 || (mis.Id != 0 && mis.UserId != AuthorizedUser.GetInstance().AuthUser.Id))
                return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
