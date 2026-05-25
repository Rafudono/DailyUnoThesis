using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public class TimelineSessionDto
{
    public string TaskName { get; set; }
    public double StartOffsetPercent { get; set; } // Где начинается блок (в % от начала дня)
    public double WidthPercent { get; set; }        // Длина блока (в % от длины дня)
    public string FormattedStart { get; set; }     // "14:00"
    public string FormattedDuration { get; set; }  // "01:20:00"
}
