using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class TimeAnalyticsSummaryDto
    {
        public string TotalWorkTime { get; set; }      // Общее время за неделю
        public string AverageSession { get; set; }     // Средняя сессия
        public int ActiveDaysCount { get; set; }       // Активные дни за неделю
        public string PeakActivityDay { get; set; }    // День пиковой активности
        public string PeakActivityTime { get; set; }   // Время пиковой активности
        public string? MotivationalNote { get; set; }   // Мотивационная заметка
    }
}
