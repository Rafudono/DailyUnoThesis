using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DailyUnoThesis.Models;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls;
public partial class MonthCalendarViewModel : ObservableObject
{
    private readonly TaskStateService _taskState = TaskStateService.GetInstance();
    private Mission _draggedMission;
    [ObservableProperty]
    private DateTime _currentMonth = DateTime.Today;
    [ObservableProperty]
    private ObservableCollection<CalendarDay> _days = new();
    public ObservableCollection<string> DayOfWeekHeaders { get; set; }

    [ObservableProperty]
    private Mission _selectedMission;

    [ObservableProperty]
    private int _dropTargetIndex = -1;
    [ObservableProperty]
    private ObservableCollection<Mission> _inboxTreeMissions = new();
    [ObservableProperty]
    private int _startHour = 9;
    [ObservableProperty]
    private int _endHour = 22;
    [ObservableProperty]
    private ObservableCollection<string> _hourLabels = new();
    [ObservableProperty]
    private ObservableCollection<int> _weekRows = new() { 0, 1, 2, 3, 4, 5 };
    [ObservableProperty]
    private double _hourSlotHeight = 18;
    [ObservableProperty]
    private double _dayHeaderHeight = 20;

    public void OnDragOver(int index)
    {
        DropTargetIndex = index;
    }

    public string MonthHeader => _currentMonth.ToString("Y", new System.Globalization.CultureInfo("ru-RU"));
    public MonthCalendarViewModel()
    {
        _taskState.TasksChanged += OnTasksChanged;

        NavigateToMonth(_currentMonth);
        InitializeDayOfWeekHeaders();
        RebuildHourLabels();

        // Асинхронная загрузка данных
        _ = LoadDataFromApi();

    }
    public ICommand PreviousMonthCommand => new RelayCommand(() =>
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        NavigateToMonth(_currentMonth);
    });

    // Команда для следующего месяца
    public ICommand NextMonthCommand => new RelayCommand(() =>
    {
        _currentMonth = _currentMonth.AddMonths(1);
        NavigateToMonth(_currentMonth);
    });
    #region Календарь (генерация и подгрузка данных)
    private void InitializeDayOfWeekHeaders()
    {
        var culture = new System.Globalization.CultureInfo("ru-RU");
        DayOfWeekHeaders = new ObservableCollection<string>();

        // Получаем сокращенные названия дней недели
        foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
        {
            var dayOfWeek = (DayOfWeek)day;
            DayOfWeekHeaders.Add(culture.DateTimeFormat.GetShortestDayName(dayOfWeek));
        }
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

        int daysFromMonday = ((int)firstDay.DayOfWeek + 6) % 7;

        var start = firstDay.AddDays(-daysFromMonday);

        // Всегда 6 недель × 7 дней = 42 дня
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

        LoadTasksForMonth();
    }
    #endregion


    private void LoadTasksForMonth()
    {
        RefreshAllDays();

    }

    internal async Task MoveMissionToDayWithInsert(CalendarDay targetDay, int insertIndex, Mission draggedMission, bool isFromInbox = false)
    {
        // Удаляем из локальных коллекций сразу для более плавного UX.
        if (isFromInbox) InboxMissions.Remove(draggedMission);
        else PlannedMissions.Remove(draggedMission);
        foreach (var day in Days) day.Tasks?.Remove(draggedMission);

        var realTasks = targetDay.Tasks?
            .Where(t => t != draggedMission)
            .OrderBy(t => t.StartDate)
            .ToList() ?? new List<Mission>();

        DateTime dayStart = targetDay.Date.Date.AddHours(StartHour);
        DateTime dayEnd = targetDay.Date.Date.AddHours(EndHour);
        TimeSpan duration = (draggedMission.EndDate - draggedMission.StartDate)?.TotalHours > 0
            ? (draggedMission.EndDate - draggedMission.StartDate).Value
            : TimeSpan.FromHours(1);

        DateTime newStartTime, newEndTime;

        if (realTasks.Count == 0)
        {
            newStartTime = dayStart;
            newEndTime = newStartTime.Add(duration);
        }
        else if (insertIndex == 0)
        {
            newStartTime = realTasks[0].StartDate.Value - duration;
            if (newStartTime < dayStart) newStartTime = dayStart;
            newEndTime = realTasks[0].StartDate.Value;
        }
        else if (insertIndex >= realTasks.Count)
        {
            newStartTime = realTasks.Last().EndDate.Value;
            newEndTime = newStartTime.Add(duration);
        }
        else
        {
            newStartTime = realTasks[insertIndex - 1].EndDate.Value;
            newEndTime = realTasks[insertIndex].StartDate.Value;
        }

        draggedMission.StartDate = newStartTime;
        draggedMission.EndDate = newEndTime > dayEnd ? dayEnd : newEndTime;

        if (draggedMission.Id == 0)
            await _taskState.AddAsync(draggedMission);
        else
            await _taskState.UpdateAsync(draggedMission);

        RebuildMissionBuckets();
        RefreshAllDays();
        SelectedMission = null;
    }
    // метод для обновления всех дней
    private void RefreshAllDays()
    {
        foreach (var day in Days)
        {
            if (day.Tasks == null)
                day.Tasks = new ObservableCollection<Mission>();
            else
                day.Tasks.Clear();

            // Показываем:
            // 1. Подзадачи (есть IdUpMission)
            // 2. Миссии-одиночки (нет IdUpMission и нет подзадач)
            var dayTasks = PlannedMissions
                .Where(t => t.StartDate.HasValue &&
                            t.StartDate.Value.Date == day.Date.Date &&
                            (t.IdUpMission != null && t.IdUpMission != 0 ||  // подзадача
                             (t.IdUpMission == null || t.IdUpMission == 0) &&
                             t.InverseIdUpMissionNavigation.Count == 0))  // одиночка
                .OrderBy(t => t.StartDate)
                .ToList();

            foreach (var task in dayTasks)
                day.Tasks.Add(task);
        }
        OnPropertyChanged(nameof(Days));
    }

    /*public bool CanInsertAt(CalendarDay targetDay, int insertIndex, Mission draggedMission)
    {
        var realTasks = targetDay.Tasks?
            .Where(t => t != draggedMission)
            .OrderBy(t => t.StartDate)
            .ToList() ?? new List<Mission>();

        // Пустой день - всегда можно вставить в начало (индекс 0)
        if (realTasks.Count == 0)
        {
            return insertIndex == 0;
        }

        DateTime dayStart = targetDay.Date.Date.AddHours(StartHour);
        DateTime dayEnd = targetDay.Date.Date.AddHours(EndHour);

        if (insertIndex == 0)
        {
            var firstTask = realTasks[0];
            return firstTask.StartDate.Value > dayStart;
        }
        if (insertIndex >= realTasks.Count)
        {
            var lastTask = realTasks[realTasks.Count - 1];
            return lastTask.EndDate.Value < dayEnd;
        }

        var taskBefore = realTasks[insertIndex - 1];
        var taskAfter = realTasks[insertIndex];
        return taskAfter.StartDate.Value > taskBefore.EndDate.Value;
    } */

    public bool CanInsertAt(CalendarDay targetDay, int insertIndex, Mission draggedMission)
    {
        // Запрещаем вставку в прошедшие дни (дата меньше сегодняшней)
        if (targetDay.Date.Date < DateTime.Today.Date)
            return false;

        // Запрещаем вставку в дни других месяцев
        if (targetDay.IsOtherMonth)
            return false;

        var realTasks = targetDay.Tasks?
            .Where(t => t != draggedMission)
            .OrderBy(t => t.StartDate)
            .ToList() ?? new List<Mission>();

        // Пустой день - можно вставить только в начало (индекс 0)
        if (realTasks.Count == 0)
        {
            return insertIndex == 0;
        }

        DateTime dayStart = targetDay.Date.Date.AddHours(9);
        DateTime dayEnd = targetDay.Date.Date.AddHours(21);

        if (insertIndex == 0)
        {
            var firstTask = realTasks[0];
            return firstTask.StartDate.Value > dayStart;
        }
        if (insertIndex >= realTasks.Count)
        {
            var lastTask = realTasks[realTasks.Count - 1];
            return lastTask.EndDate.Value < dayEnd;
        }

        var taskBefore = realTasks[insertIndex - 1];
        var taskAfter = realTasks[insertIndex];
        return taskAfter.StartDate.Value > taskBefore.EndDate.Value;
    }
    public async Task LoadDataFromApi()
    {
        try
        {
            await _taskState.LoadAsync();
            RebuildMissionBuckets();
            RefreshAllDays();
        }
        catch (Exception ex)
        {
            ;
        }
    }
    private void OnTasksChanged(object? sender, TaskStateChangedEventArgs e)
    {
        RebuildMissionBuckets();
        RefreshAllDays();
    }

    private void RebuildMissionBuckets()
    {
        PlannedMissions.Clear();
        InboxMissions.Clear();
        InboxTreeMissions.Clear();

        foreach (var mission in _taskState.Tasks)
        {
            if (mission.StartDate != null && mission.EndDate != null)
                PlannedMissions.Add(mission);
            else
                InboxMissions.Add(mission);
        }

        BuildInboxTreeMissions();
    }

    private void BuildInboxTreeMissions()
    {
        var roots = _taskState.Tasks
            .Where(IsRootMission)
            .Where(root => ContainsMissionWithoutDeadlines(root, new HashSet<int>()))
            .ToList();

        foreach (var root in roots)
        {
            InboxTreeMissions.Add(root);
        }
    }

    private static bool IsRootMission(Mission mission)
    {
        return mission.IdUpMission == null || mission.IdUpMission == 0;
    }

    private static bool ContainsMissionWithoutDeadlines(Mission mission, HashSet<int> visited)
    {
        if (!visited.Add(mission.Id))
            return false;

        if (!mission.StartDate.HasValue && !mission.EndDate.HasValue)
            return true;

        if (mission.InverseIdUpMissionNavigation == null)
            return false;

        foreach (var child in mission.InverseIdUpMissionNavigation)
        {
            if (ContainsMissionWithoutDeadlines(child, visited))
                return true;
        }

        return false;
    }

    partial void OnStartHourChanged(int value)
    {
        if (value >= EndHour)
            EndHour = Math.Min(value + 1, 23);

        RebuildHourLabels();
    }

    partial void OnEndHourChanged(int value)
    {
        if (value <= StartHour)
            StartHour = Math.Max(value - 1, 0);

        RebuildHourLabels();
    }

    private void RebuildHourLabels()
    {
        HourLabels.Clear();
        for (int hour = StartHour; hour <= EndHour; hour++)
        {
            HourLabels.Add($"{hour:D2}:00");
        }

        OnPropertyChanged(nameof(DayTimelineHeight));
        OnPropertyChanged(nameof(DayCellHeight));
    }

    public double DayTimelineHeight => HourLabels.Count * HourSlotHeight;
    public double DayCellHeight => DayHeaderHeight + DayTimelineHeight;

    public ObservableCollection<Mission> AllMissions => _taskState.Tasks;
    public ObservableCollection<Mission> PlannedMissions { get; set; } = new ObservableCollection<Mission>();
    public ObservableCollection<Mission> InboxMissions { get; set; } = new ObservableCollection<Mission>();
}
