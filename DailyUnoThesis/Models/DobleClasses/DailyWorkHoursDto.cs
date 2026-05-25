using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses
{
   public  class DailyWorkHoursDto
    {
        public int Index { get; set; }
        public string DayOfWeek { get; set; } // Например, "Понедельник", "Пн"
        public string TotalWorkTime { get; set; } // Форматированное время: "08:30:00"
        public TimeSpan RawWorkTime { get; set; } // Неформатированное время (для внутренних расчетов UI)

        public double? NormalizedHeight { get; set; }
        public double? NormalizedWidth { get; set; }
    }
}
