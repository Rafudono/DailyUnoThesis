using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public class TimelineSegmentDto
{
    public string TaskName { get; set; }
    public double Width { get; set; }        // Ширина в пикселях
    public bool IsTask { get; set; }         // Это задача или пустой промежуток?
    public string ToolTipText { get; set; }

    public double DurationMinutes { get; set; } // Длительность в минутах
    public string ToolTipContent { get; set; }  // Готовый текст для подсказки
}
