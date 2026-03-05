using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models;
public class TaskMenuSelector : DataTemplateSelector
{
    public DataTemplate ItemTemplate { get; set; }
    public DataTemplate HeaderTemplate { get; set; }
    public DataTemplate SeparatorTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is NavMenuItem menu)
        {
            if (menu.IsSeparator) return SeparatorTemplate;
            if (menu.IsHeader) return HeaderTemplate;
            return ItemTemplate;
        }
        return base.SelectTemplateCore(item);
    }
}
