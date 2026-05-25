using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace DailyUnoThesis.Styles.Dictionary;
public class ButtonHelper
{
    // Свойство для хранения иконки (глифа)
    public static readonly DependencyProperty IconGlyphProperty =
        DependencyProperty.RegisterAttached("IconGlyph", typeof(string), typeof(ButtonHelper), new PropertyMetadata(default(string)));

    public static string GetIconGlyph(DependencyObject obj) => (string)obj.GetValue(IconGlyphProperty);
    // Исправлено: добавлен параметр 'value'
    public static void SetIconGlyph(DependencyObject obj, string value) => obj.SetValue(IconGlyphProperty, value);

    // Свойство для цвета при наведении (Hover)
    public static readonly DependencyProperty HoverColorProperty =
        DependencyProperty.RegisterAttached("HoverColor", typeof(Brush), typeof(ButtonHelper), new PropertyMetadata(default(Brush)));

    public static Brush GetHoverColor(DependencyObject obj) => (Brush)obj.GetValue(HoverColorProperty);
    // Исправлено: добавлен параметр 'value'
    public static void SetHoverColor(DependencyObject obj, Brush value) => obj.SetValue(HoverColorProperty, value);
}
