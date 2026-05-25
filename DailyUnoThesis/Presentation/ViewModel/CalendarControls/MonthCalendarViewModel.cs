using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DailyUnoThesis.Models;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
//using Java.Util;

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
    private TaskCompletionTime _selectedSession;

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
       // _ = LoadDataFromApi();

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

    internal async Task MoveMissionToDayWithInsert(CalendarDay targetDay, int insertIndex, TaskCompletionTime draggedSession, bool isFromInbox = false)
    {
        if (draggedSession?.IdMissionNavigation == null) return;

        var mission = draggedSession.IdMissionNavigation;

        // Сохраняем все сессии миссии (включая другие дни)
        var allMissionSessions = GetAllMissionSessions(mission);

        // Удаляем эту сессию из всех дней
        foreach (var day in Days)
        {
            var existingSession = day.TasksSessions.FirstOrDefault(s => s.Id == draggedSession.Id);
            if (existingSession != null)
                day.TasksSessions.Remove(existingSession);
        }
        if (mission.TaskCompletionTimes?.Contains(draggedSession) == true)
            mission.TaskCompletionTimes.Remove(draggedSession);
        // Получаем сессии этого дня (кроме перетаскиваемой)
        var existingSessions = targetDay.TasksSessions
            .Where(s => s.Id != draggedSession.Id && s.IdMission == mission.Id)
            .OrderBy(s => s.StartExecution)
            .ToList();

        DateTime dayStart = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour);
        DateTime dayEnd = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour);

        // Длительность из миссии или сессии
        TimeSpan duration;
        if (mission.DurationMinutes.HasValue && mission.DurationMinutes.Value > 0)
            duration = TimeSpan.FromMinutes(mission.DurationMinutes.Value);
        else if (draggedSession.StartExecution.HasValue && draggedSession.EndExecution.HasValue)
            duration = draggedSession.EndExecution.Value - draggedSession.StartExecution.Value;
        else
            duration = TimeSpan.FromHours(1);

        DateTime newStartTime, newEndTime;

        // Расчет позиции вставки
        if (existingSessions.Count == 0)
        {
            newStartTime = dayStart;
            newEndTime = newStartTime.Add(duration);
        }
        else if (insertIndex == 0)
        {
            newStartTime = existingSessions[0].StartExecution.Value - duration;
            if (newStartTime < dayStart) newStartTime = dayStart;
            newEndTime = existingSessions[0].StartExecution.Value;
        }
        else if (insertIndex >= existingSessions.Count)
        {
            newStartTime = existingSessions.Last().EndExecution.Value;
            newEndTime = newStartTime.Add(duration);
        }
        else
        {
            newStartTime = existingSessions[insertIndex - 1].EndExecution.Value;
            newEndTime = existingSessions[insertIndex].StartExecution.Value;
        }

        // Корректируем границы дня
        if (newEndTime > dayEnd) newEndTime = dayEnd;
        if (newStartTime < dayStart) newStartTime = dayStart;

        // Обновляем только эту сессию
        draggedSession.StartExecution = newStartTime;
        draggedSession.EndExecution = newEndTime;

        // Обновляем общие даты миссии на основе ВСЕХ сессий
        var last=UpdateMissionDatesFromSessions(mission);

        // Сохраняем в БД
        await _taskState.UpdateAsync(last);

        // Обновляем отображение
        RebuildMissionBuckets();
        RefreshAllDays();
        SelectedSession = null;
    }

    // Вспомогательный метод для обновления дат миссии на основе всех её сессий
    private Mission UpdateMissionDatesFromSessions(Mission mission)
    {
        if (mission.TaskCompletionTimes == null || !mission.TaskCompletionTimes.Any())
        {
            mission.StartDate = null;
            mission.EndDate = null;
            return mission;
        }

        // Находим самую раннюю и самую позднюю сессию
        var minStart = mission.TaskCompletionTimes
            .Where(s => s.StartExecution.HasValue)
            .Min(s => s.StartExecution.Value);

        var maxEnd = mission.TaskCompletionTimes
            .Where(s => s.EndExecution.HasValue)
            .Max(s => s.EndExecution.Value);

        mission.StartDate = minStart;
        mission.EndDate = maxEnd;
        return mission;
    }

    // Получение всех сессий миссии (из всех дней)
    private List<TaskCompletionTime> GetAllMissionSessions(Mission mission)
    {
        var sessions = new List<TaskCompletionTime>();

        // Собираем из всех дней календаря
        foreach (var day in Days)
        {
            var daySessions = day.TasksSessions
                .Where(s => s.IdMission == mission.Id)
                .ToList();
            sessions.AddRange(daySessions);
        }

        // Если в днях не нашли, берем из самой миссии
        if (!sessions.Any() && mission.TaskCompletionTimes != null)
            sessions.AddRange(mission.TaskCompletionTimes);

        return sessions;
    }

    // метод для обновления всех дней
    public void RefreshAllDays()
    {
        foreach (var day in Days)
        {
            if (day.Tasks == null)
                day.TasksSessions = new ObservableCollection<TaskCompletionTime>();
            else
                day.TasksSessions.Clear();

            // Показываем:
            // 1. Подзадачи (есть IdUpMission)
            // 2. Миссии-одиночки (нет IdUpMission и нет подзадач)
            var dayTasks = PlannedMissions
                .Where(t => t.StartDate.HasValue &&
                            t.IsProject is not true &&                        //проекты не отображаю на календаре
                            (t.IdUpMission != null && t.IdUpMission != 0 ||  // подзадача
                             (t.IdUpMission == null || t.IdUpMission == 0) &&
                             t.InverseIdUpMissionNavigation.Count == 0))  // одиночка
                .OrderBy(t => t.StartDate)
                .ToList();
            var allSessions = new List<TaskCompletionTime>();

            foreach (var task in dayTasks)      //добаввляю в день сессии выполнения задач
                if (task.TaskCompletionTimes != null && task.TaskCompletionTimes.Any())
                {
                    var sessionsForDay = task.TaskCompletionTimes
                        .Where(session => session.StartExecution.HasValue &&
                                          session.StartExecution.Value.Date == day.Date.Date)
                        .ToList();
                    allSessions.AddRange(sessionsForDay);
                }

            foreach (var session in allSessions.OrderBy(s => s.StartExecution))     //сортирую сессии внутри дня по времени начала
                day.TasksSessions.Add(session);
        }
        OnPropertyChanged(nameof(Days));
    }

     internal bool CanInsertSessionAt(CalendarDay targetDay, int insertIndex, TaskCompletionTime draggedSession)
    {
        if (targetDay.Date.Date < DateTime.Today.Date)
            return false;

        var mission = draggedSession.IdMissionNavigation;
        var duration = draggedSession.EndExecution.Value - draggedSession.StartExecution.Value;

        var realTasks = targetDay.TasksSessions?
            .Where(t => t != draggedSession)
            .OrderBy(t => t.StartExecution)
            .ToList() ?? new List<TaskCompletionTime>();

        DateTime dayStart = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour);
        DateTime dayEnd = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour);

        // Пустой день
        if (realTasks.Count == 0)
        {
            return insertIndex == 0 && duration <= (dayEnd - dayStart);
        }

        if (insertIndex == 0)
        {
            var availableTime = realTasks[0].StartExecution.Value - dayStart;
            return duration <= availableTime;
        }

        if (insertIndex >= realTasks.Count)
        {
            var availableTime = dayEnd - realTasks.Last().EndExecution.Value;
            return duration <= availableTime;
        }

        var gap = realTasks[insertIndex].StartExecution.Value - realTasks[insertIndex - 1].EndExecution.Value;
        return duration <= gap;
    }

    internal bool CanInsertMissionAt(CalendarDay targetDay, int insertIndex, Mission draggedMission)
    {

        if (targetDay.Date.Date < DateTime.Today.Date)
            return false;

        var realTasks = targetDay.TasksSessions?
            .OrderBy(t => t.StartExecution)
            .ToList() ?? new List<TaskCompletionTime>();

        // Пустой день - можно вставить только в начало (индекс 0)
        if (realTasks.Count == 0)
        {
            return insertIndex == 0;
        }

        DateTime dayStart = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour);
        DateTime dayEnd = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour);

        var duration = draggedMission.DurationMinutes > 0
            ? TimeSpan.FromMinutes(draggedMission.DurationMinutes.Value)
            : TimeSpan.FromHours(1);

        if (insertIndex == 0)
        {
            return realTasks[0].StartExecution.Value - duration >= dayStart;
        }
        if (insertIndex >= realTasks.Count)
        {
            return realTasks.Last().EndExecution.Value + duration <= dayEnd;
        }

        var gap = realTasks[insertIndex].StartExecution.Value - realTasks[insertIndex - 1].EndExecution.Value;
        return gap >= duration;
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
    internal void OnTasksChanged(object? sender, TaskStateChangedEventArgs e)
    {
        RebuildMissionBuckets();
        RefreshAllDays();
    }

    public void RebuildMissionBuckets()
    {
        PlannedMissions.Clear();
        InboxMissions.Clear();
        InboxTreeMissions.Clear();

        foreach (var mission in _taskState.Tasks)
        {
            if (IsPlannedMission(mission))
                PlannedMissions.Add(mission);
            else
                InboxMissions.Add(mission);
        }

        BuildInboxTreeMissions();
    }

    /// <summary>
    /// Совпадает с бакетом «запланировано»: обе даты заданы (как в RebuildMissionBuckets).
    /// </summary>
    private static bool IsPlannedMission(Mission mission)       //запланирована ли миссия
    {
        if (mission.StartDate != null && mission.EndDate != null&& mission.StartDate!= DateTime.MinValue&& mission.EndDate!= DateTime.MinValue)  //назначили дедлайн
        {
            if (mission.StartDate.Value.Date == mission.EndDate.Value.Date)  //можно ли выполнить задачу за 1 день
                return true;
            else if (mission.TaskCompletionTimes is not null&&mission.TaskCompletionTimes.Count() > 0)  
                //если нет нужно проверить расплнирована ли она полностью на разные даты (1 запись TaskCompletionTimes создается при планировании на 1 сессию выполнения)
                return true;
            else                //если окажется, что нет, то нужно дать пользователю ее раскидать по дням в календаре => можно в inbox
                return false;
        }
        else
            return false;
    }

    /// <summary>
    /// Inbox: всё, что не полностью запланировано (нет начала и/или конца).
    /// Должно совпадать с веткой else в RebuildMissionBuckets.
    /// </summary>
    private static bool IsInboxMission(Mission mission)
    {
        return !IsPlannedMission(mission);
    }

    private void BuildInboxTreeMissions()
    {
        foreach (var root in _taskState.Tasks.Where(IsRootMission))
        {
            if (SubtreeContainsInboxMission(root, new HashSet<int>()))
                InboxTreeMissions.Add(root);
        }
    }

    private static bool IsRootMission(Mission mission)
    {
        return mission.IdUpMission == null || mission.IdUpMission == 0;
    }

    private static bool SubtreeContainsInboxMission(Mission mission, HashSet<int> visited)
    {
        if (mission.Id > 0 && !visited.Add(mission.Id))
            return false;

        if (IsInboxMission(mission))
            return true;

        if (mission.InverseIdUpMissionNavigation == null || mission.InverseIdUpMissionNavigation.Count == 0)
            return false;

        foreach (var child in mission.InverseIdUpMissionNavigation)
        {
            if (SubtreeContainsInboxMission(child, visited))
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

    internal async Task MoveSessionToDay(CalendarDay targetDay, int insertIndex, TaskCompletionTime session)
    {
        if (session?.IdMissionNavigation == null) return;

        var mission = session.IdMissionNavigation;

        // Удаляем сессию из текущего дня
        foreach (var day in Days)
        {
            var existing = day.TasksSessions.FirstOrDefault(s => s.Id == session.Id);
            if (existing != null)
                day.TasksSessions.Remove(existing);
        }

        // Рассчитываем новое время
        var dayStart = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour);
        var dayEnd = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour);

        var duration = session.EndExecution.Value - session.StartExecution.Value;

        var existingSessions = targetDay.TasksSessions
            .Where(s => s.Id != session.Id)
            .OrderBy(s => s.StartExecution)
            .ToList();

        DateTime newStartTime;

        if (existingSessions.Count == 0)
        {
            newStartTime = dayStart;
        }
        else if (insertIndex == 0)
        {
            newStartTime = dayStart;
        }
        else if (insertIndex >= existingSessions.Count)
        {
            newStartTime = existingSessions.Last().EndExecution.Value;
        }
        else
        {
            newStartTime = existingSessions[insertIndex - 1].EndExecution.Value;
        }

        var newEndTime = newStartTime.Add(duration);

        // Корректируем границы дня
        //if (newEndTime > dayEnd) newEndTime = dayEnd;
        //if (newStartTime < dayStart) newStartTime = dayStart;

        // Обновляем сессию
        session.StartExecution = newStartTime;
        session.EndExecution = newEndTime;

        // Обновляем даты миссии
       var last= UpdateMissionDatesFromSessions(mission);

        // Сохраняем
        await _taskState.UpdateAsync(last);

        // Обновляем отображение
        RefreshAllDays();
        SelectedSession = null;
    }

    internal async Task CreateSessionFromMission(CalendarDay targetDay, int insertIndex, Mission mission)
    {
        // Получаем длительность
        var duration = mission.DurationMinutes > 0
            ? TimeSpan.FromMinutes(mission.DurationMinutes.Value)
            : TimeSpan.FromHours(1);

        // Удаляем миссию из Inbox
        InboxMissions.Remove(mission);

        // Рассчитываем время вставки
        var dayStart = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour);
        var dayEnd = targetDay.Date.Date.AddHours(AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour);

        var existingSessions = targetDay.TasksSessions
            .OrderBy(s => s.StartExecution)
            .ToList();

        DateTime newStartTime;

        if (existingSessions.Count == 0)
        {
            newStartTime = dayStart;
        }
        else if (insertIndex == 0)
        {
            newStartTime = dayStart;
        }
        else if (insertIndex >= existingSessions.Count)
        {
            newStartTime = existingSessions.Last().EndExecution.Value;
        }
        else
        {
            newStartTime = existingSessions[insertIndex - 1].EndExecution.Value;
        }

        var newEndTime = newStartTime.Add(duration);

        // Корректируем, если выходит за границы дня
        //if (newEndTime > dayEnd)
        //    newEndTime = dayEnd;
        //нихрена подобного, если задача длиннее дня предлагаем создать новую сессию для ее выполнения
        

        // Создаем новую сессию
        var newSession = new TaskCompletionTime
        {
            IdMission = mission.Id,
            StartExecution = newStartTime,
            EndExecution = newEndTime,
            IdMissionNavigation = mission
        };

        if (mission.TaskCompletionTimes == null)
            mission.TaskCompletionTimes = new List<TaskCompletionTime>();

        mission.TaskCompletionTimes.Add(newSession);

        // Обновляем даты миссии
        //mission.StartDate = newStartTime;
        //mission.EndDate = newEndTime;
        //ЭТА СЕССИЯ МОЖЕТ НЕ ЕДИНСТВЕННОЙ БЫТЬ, А ПРОМЕЖУТОЧНОЙ ТАК НЕЛЬЗЯ
        UpdateMissionDatesFromSessions(mission);
        // Сохраняем в БД
        if (mission.Id == 0)
            await _taskState.AddAsync(mission);
        else
            await _taskState.UpdateAsync(mission);

        // Обновляем отображение
        RebuildMissionBuckets();
        RefreshAllDays();
    }

    public double DayTimelineHeight => HourLabels.Count * HourSlotHeight;
    public double DayCellHeight => DayHeaderHeight + DayTimelineHeight;

    public ObservableCollection<Mission> PlannedMissions { get; set; } = new ObservableCollection<Mission>();
    public ObservableCollection<Mission> InboxMissions { get; set; } = new ObservableCollection<Mission>();
}
   //internal async Task MoveMissionToDayWithInsert(CalendarDay targetDay, int insertIndex, Mission draggedMission, bool isFromInbox = false)
   // {
   //     // Удаляем из локальных коллекций сразу для более плавного UX.
   //     if (isFromInbox) InboxMissions.Remove(draggedMission);
   //     else PlannedMissions.Remove(draggedMission);
   //     foreach (var day in Days) day.Tasks?.Remove(draggedMission);

   //     var realTasks = targetDay.Tasks?
   //         .Where(t => t != draggedMission)
   //         .OrderBy(t => t.StartDate)
   //         .ToList() ?? new List<Mission>();

   //     DateTime dayStart = targetDay.Date.Date.AddHours(StartHour);
   //     DateTime dayEnd = targetDay.Date.Date.AddHours(EndHour);
   //     TimeSpan duration = (draggedMission.EndDate - draggedMission.StartDate)?.TotalHours > 0
   //         ? (draggedMission.EndDate - draggedMission.StartDate).Value
   //         : TimeSpan.FromHours(1);

   //     DateTime newStartTime, newEndTime;

   //     if (realTasks.Count == 0)
   //     {
   //         newStartTime = dayStart;
   //         newEndTime = newStartTime.Add(duration);
   //     }
   //     else if (insertIndex == 0)
   //     {
   //         newStartTime = realTasks[0].StartDate.Value - duration;
   //         if (newStartTime < dayStart) newStartTime = dayStart;
   //         newEndTime = realTasks[0].StartDate.Value;
   //     }
   //     else if (insertIndex >= realTasks.Count)
   //     {
   //         newStartTime = realTasks.Last().EndDate.Value;
   //         newEndTime = newStartTime.Add(duration);
   //     }
   //     else
   //     {
   //         newStartTime = realTasks[insertIndex - 1].EndDate.Value;
   //         newEndTime = realTasks[insertIndex].StartDate.Value;
   //     }

   //     draggedMission.StartDate = newStartTime;
   //     draggedMission.EndDate = newEndTime > dayEnd ? dayEnd : newEndTime;

   //     if (draggedMission.Id == 0)
   //         await _taskState.AddAsync(draggedMission);
   //     else
   //         await _taskState.UpdateAsync(draggedMission);

   //     RebuildMissionBuckets();
   //     RefreshAllDays();
   //     SelectedMission = null;
   // }
