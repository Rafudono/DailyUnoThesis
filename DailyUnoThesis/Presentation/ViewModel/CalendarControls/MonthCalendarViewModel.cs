using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls;
public partial class MonthCalendarViewModel : ObservableObject
{
    private readonly APIHost _api = APIHost.GetInstance();
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

    public void OnDragOver(int index)
    {
        DropTargetIndex = index;
    }

    public string MonthHeader => _currentMonth.ToString("Y", new System.Globalization.CultureInfo("ru-RU"));
    public MonthCalendarViewModel()
    {

        NavigateToMonth(_currentMonth);
        InitializeDayOfWeekHeaders();

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
        // Удаляем из источника
        if (isFromInbox)
            InboxMissions.Remove(draggedMission);
        else
        {
            PlannedMissions.Remove(draggedMission);
            foreach (var day in Days)
                day.Tasks?.Remove(draggedMission);
        }

        var realTasks = targetDay.Tasks?
            .Where(t => t != draggedMission)
            .OrderBy(t => t.StartDate)
            .ToList() ?? new List<Mission>();

        DateTime dayStart = targetDay.Date.Date.AddHours(9);
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
        draggedMission.EndDate = newEndTime;

        // Сохраняем в API
        if (draggedMission.Id == 0)
            await APIHost.GetInstance().CreateMission(draggedMission);
        else
            await APIHost.GetInstance().EditMission(draggedMission);

        PlannedMissions.Add(draggedMission);
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
            // Загружаем миссии пользователя
            var missions = await _api.GetMissions();
            if (missions != null && missions.Any())
            {
                PlannedMissions.Clear();
                InboxMissions.Clear();
                foreach (var m in missions)
                {
                    if(m.StartDate !=null&& m.EndDate !=null)
                        PlannedMissions.Add(m);
                    else InboxMissions.Add(m);
                }
            }

            // Загружаем категории (если нужны)
            var categories = await _api.GetCategories();

            RefreshAllDays();
        }
        catch (Exception ex)
        {
            ;
        }
    }
    public ObservableCollection<Mission> PlannedMissions { get; set; } = new ObservableCollection<Mission>();
    public ObservableCollection<Mission> InboxMissions { get; set; } = new ObservableCollection<Mission>();
}
