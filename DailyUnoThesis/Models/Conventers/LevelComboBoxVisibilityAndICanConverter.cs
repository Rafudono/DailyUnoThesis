using System;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Models.MainClasses;
using Microsoft.UI.Xaml.Data;

namespace DailyUnoThesis.Models.Conventers;

public class LevelComboBoxVisibilityAndICanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null) return null;
        if (value is  Mission mis)
        {
            if (mis.Id != 0)
            {
                if ((mis.LevelUp == 1) && (mis.UserId == AuthorizedUser.GetInstance().AuthUser.Id))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
                return Visibility.Visible;

        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

}
