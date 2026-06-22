using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;

public class DashboardDto
{
    public string? UserName { get; set; }
    public int TasksTodayCount { get; set; }     // Задачи на сегодня (невыполненные)
    public int ActiveTasksCount { get; set; }    // Всего задач в работе
    public int CompletedTasksCount { get; set; } // Выполненные за последние 7 дней
    public string FocusTimeText { get; set; }    // Сумма времени таймера за сегодня (чч:мм:сс)

    public List<DashboardMissionDto> ActiveMissions { get; set; } = new();
    public List<DashboardProjectDto> ActiveProjects { get; set; } = new();
}

public class DashboardMissionDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? ProjectName { get; set; }
    public string DueDateText { get; set; }
}

public class DashboardProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public double ProgressValue { get; set; }
    public string ProgressText => $"{ProgressValue}%";
    public string StatusText { get; set; }
}
