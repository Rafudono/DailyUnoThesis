using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public class DayTimelineDto
{
    public string DayName { get; set; } // Пн, Вт...
    public DateTime Date { get; set; }
    public List<TimelineSessionDto>? Sessions { get; set; }
    public List<TimelineSegmentDto>? Segments { get; set; } = new List<TimelineSegmentDto>();
}
