using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public class TaskDistributionDto
{
    public string TaskName { get; set; }        // Название задачи
    public TimeSpan TotalTime { get; set; }     // Общее время (TimeSpan)
    public string FormattedTime { get; set; }   // "12:30:00"
    public double Percentage { get; set; }      // Доля в процентах (0-100)
    public string ToolTipContent { get; set; }
}
