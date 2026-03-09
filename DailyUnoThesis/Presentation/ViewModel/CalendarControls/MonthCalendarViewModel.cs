using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls;
public class MonthCalendarViewModel : ObservableObject
{
    private DateTime _currentMonth = DateTime.Today;
    public ObservableCollection<CalendarDay> Days { get; set; } = new();

    public string MonthHeader => _currentMonth.ToString("Y", new System.Globalization.CultureInfo("ru-RU"));
    public MonthCalendarViewModel()
    {
        NavigateToMonth(_currentMonth);
    }
    public void NavigateToMonth(DateTime month)
    {
        _currentMonth = new DateTime(month.Year, month.Month, 1);
        GenerateDays();
        OnPropertyChanged(nameof(MonthHeader));
    }

    private void GenerateDays()
    {
        Days.Clear();

        var firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
        //начало с пн
        int diff = (int)firstDay.DayOfWeek - 1;  
        if (diff < 0) diff += 7;
        var start = firstDay.AddDays(-diff);

        for (int i = 0; i < 42; i++)
        {
            var date = start.AddDays(i);
            var isOtherMonth = date.Month != _currentMonth.Month;
            var isToday = date.Date == DateTime.Today;

            var day = new CalendarDay
            {
                Date = date,
                IsOtherMonth = isOtherMonth,
                IsToday = isToday
            };

            Days.Add(day);
        }

        // Здесь вы загружаете задачи (из сервиса, базы и т.д.)
        LoadTasksForMonth();
    }

    private void LoadTasksForMonth()
    {
        foreach (var day in Days)
        {
            var dayTasks = PlannedMissions.Where(t => t.StartDate.Value.Date == day.Date.Date).ToList();
            day.Tasks = dayTasks;
        }
        Days = new ObservableCollection<CalendarDay>(Days);
    }

    // Замените на ваш источник данных
    private List<Mission> GetTasksForRange(DateTime from, DateTime to)
    {
        // Например: return _taskService.GetTasksBetween(from, to);
        return new List<Mission>(); // заглушка
    }

    public List<Mission> PlannedMissions { get; set; } = new List<Mission>()
    {
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 2, 18, 30, 0), EndDate= new DateTime(2026, 3, 2, 19,30,0) },
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 2, 18, 30, 0), EndDate= new DateTime(2026, 3, 2, 19,30,0) },
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 3, 18, 30, 0), EndDate= new DateTime(2026, 3, 3, 19,30,0) },
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 3, 18, 30, 0), EndDate= new DateTime(2026, 3, 3, 19,30,0) },
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 3, 18, 30, 0), EndDate= new DateTime(2026, 3, 3, 19,30,0) },
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 10, 18, 30, 0), EndDate= new DateTime(2026, 3, 10, 19,30,0) },
        new Mission() { Title="task", StartDate= new DateTime(2026, 3, 10, 18, 30, 0), EndDate= new DateTime(2026, 3, 10, 19,30,0) },
    };
    public List<Mission> InboxMissions { get; set; } = new List<Mission>()
    {
        new Mission () { Title="inbox"},
        new Mission () { Title="inbox"},
        new Mission () { Title="inbox"},
        new Mission () { Title="inbox"},
    };
}
