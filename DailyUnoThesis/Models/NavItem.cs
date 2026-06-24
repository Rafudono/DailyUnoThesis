using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyUnoThesis.Models;
public class NavItem
{
    public string Label { get; set; }
    public Symbol Icon { get; set; }
    public Type Page { get; set; }
}

public class IconItem
{
    public string Glyph { get; set; } // Для отображения (например, "\uE814")
    public string Value { get; set; } // Для сохранения в БД (например, "E814")
}
