using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public class TaskDeepAnalysisDto
{
    public string TaskName { get; set; }
    public string TotalTimeFormatted { get; set; }  // Общее время (05:20:10)
    public double GlobalPercentage { get; set; }    // % от всех задач за период
    public string LongestSession { get; set; }      // Самая долгая сессия
    public string ShortestSession { get; set; }     // Самая короткая
    public string AverageSession { get; set; }      // Средняя
    public double ProductivityRatio { get; set; }   // Коэффициент концентрации (0-100)
    public string PeakTimeOfDay { get; set; }       // Частое время (напр. "14:00 - 16:00")
    public int TotalSessions { get; set; }          // Кол-во подходов

    public string GlobalPercentageFormatted { get; set; } // Новое свойство
    public string ProductivityRatioFormatted { get; set; } // Новое свойство
    public string GlobalTotalTimeFormatted { get; set; } // Общее время всех задач за период
}
