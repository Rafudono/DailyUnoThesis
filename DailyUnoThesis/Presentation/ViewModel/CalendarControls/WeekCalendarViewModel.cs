using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls;

public partial class WeekCalendarViewModel : ObservableObject
{
    private readonly TaskStateService _taskState = TaskStateService.GetInstance();

    [ObservableProperty]
    private DateTime _currentWeekStart = GetWeekStart(DateTime.Today);

    //[ObservableProperty]
    //private ObservableCollection<string> _hourLabels = new();
    [ObservableProperty]
    private ObservableCollection<HourSlot> _hours = new();
    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _sessions = new();

    [ObservableProperty]
    private User _authUser;

    [ObservableProperty]
    private double _hourSlotHeight = 20;

    // Коллекции для каждого дня недели
    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _mondaySessions = new();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _tuesdaySessions = new();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _wednesdaySessions = new();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _thursdaySessions = new();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _fridaySessions = new();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _saturdaySessions = new();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _sundaySessions = new();

    public string MondayDate => CurrentWeekStart.AddDays(0).Day.ToString();
    public string TuesdayDate => CurrentWeekStart.AddDays(1).Day.ToString();
    public string WednesdayDate => CurrentWeekStart.AddDays(2).Day.ToString();
    public string ThursdayDate => CurrentWeekStart.AddDays(3).Day.ToString();
    public string FridayDate => CurrentWeekStart.AddDays(4).Day.ToString();
    public string SaturdayDate => CurrentWeekStart.AddDays(5).Day.ToString();
    public string SundayDate => CurrentWeekStart.AddDays(6).Day.ToString();

    public string WeekHeader
    {
        get
        {
            var end = CurrentWeekStart.AddDays(6);
            if (CurrentWeekStart.Month == end.Month)
                return $"{CurrentWeekStart:MMMM yyyy}";
            return $"{CurrentWeekStart:MMMM} - {end:MMMM yyyy}";
        }
    }

    public event EventHandler<double> ScrollToCurrentTimeRequested;

    public WeekCalendarViewModel()
    {
        AuthUser = AuthorizedUser.GetInstance().AuthUser;
        _taskState.TasksChanged += OnTasksChanged;
        InitializeHourLabels();
        LoadWeekData();
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    private void InitializeHourLabels()
    {
        Hours.Clear();
        for (int hour = 0; hour <= 23; hour++)  // теперь все 24 часа
        {
            Hours.Add(new HourSlot { Hour = hour });
        }
    }

    private async Task LoadWeekData()
    {
        var tasks = await APIHost.GetInstance().GetSessions();
        Sessions = new();
        Sessions.AddRange(tasks);

        BuildTimeSlots();
        //PopulateSessions();

        // Автопрокрутка к текущему времени
        var now = DateTime.Now;
        if (now >= CurrentWeekStart && now <= CurrentWeekStart.AddDays(7))
        {
            //var minutesFromStart = (now.Hour - AuthUser.DayStartTime.Value.Hour) * 60 + now.Minute;
            //var minutesFromStart = (now.Hour - 9) * 60 + now.Minute;
            //var scrollOffset = (minutesFromStart / 60.0) * HourSlotHeight;
            //ScrollToCurrentTimeRequested?.Invoke(this, scrollOffset);
        }
        OnPropertyChanged(nameof(MondayDate));
        OnPropertyChanged(nameof(TuesdayDate));
        OnPropertyChanged(nameof(WednesdayDate));
        OnPropertyChanged(nameof(ThursdayDate));
        OnPropertyChanged(nameof(FridayDate));
        OnPropertyChanged(nameof(SaturdayDate));
        OnPropertyChanged(nameof(SundayDate));
        OnPropertyChanged(nameof(WeekHeader));
    }

    private void BuildTimeSlots()
    {
        MondaySessions.Clear();
        TuesdaySessions.Clear();
        WednesdaySessions.Clear();
        ThursdaySessions.Clear();
        FridaySessions.Clear();
        SaturdaySessions.Clear();
        SundaySessions.Clear();

        for (int day = 0; day < 7; day++)
        {
            var weekDaySessions = Sessions.Where(s => s.StartExecution.Value.Date == CurrentWeekStart.AddDays(day)).ToList();
            if (day == 0)
                MondaySessions.AddRange(weekDaySessions);
            if (day == 1)
                TuesdaySessions.AddRange(weekDaySessions);
            if (day == 2)
                WednesdaySessions.AddRange(weekDaySessions);
            if (day == 3)
                ThursdaySessions.AddRange(weekDaySessions);
            if (day == 4)
                FridaySessions.AddRange(weekDaySessions);
            if (day == 5)
                SaturdaySessions.AddRange(weekDaySessions);
            if (day == 6)
                SundaySessions.AddRange(weekDaySessions);

        }
    }
    public ICommand PreviousWeekCommand => new RelayCommand(() =>
    {
        CurrentWeekStart = CurrentWeekStart.AddDays(-7);
        LoadWeekData();
    });

    public ICommand NextWeekCommand => new RelayCommand(() =>
    {
        CurrentWeekStart = CurrentWeekStart.AddDays(7);
        LoadWeekData();
    });

    public ICommand TodayCommand => new RelayCommand(() =>
    {
        CurrentWeekStart = GetWeekStart(DateTime.Today);
        LoadWeekData();
    });

    public bool CanDropAtTimeSlot(DateTime targetDay, int hour, object draggedItem)
    {
        if (targetDay.Date < DateTime.Today.Date)
            return false;

        return draggedItem is TaskCompletionTime or Mission;
    }

    public async void DropAtTimeSlot(DateTime targetDay, int hour, object draggedItem)
    {
        var startTime = targetDay.Date.AddHours(hour);
        var duration = TimeSpan.FromHours(1);
        var endTime = startTime.Add(duration);

        if (draggedItem is TaskCompletionTime session)
        {
            session.StartExecution = startTime;
            session.EndExecution = endTime;
            await _taskState.UpdateAsync(session.IdMissionNavigation);
        }
        else if (draggedItem is Mission mission)
        {
            var newSession = new TaskCompletionTime
            {
                IdMission = mission.Id,
                StartExecution = startTime,
                EndExecution = endTime,
                IdMissionNavigation = mission
            };

            mission.TaskCompletionTimes ??= new List<TaskCompletionTime>();
            mission.TaskCompletionTimes.Add(newSession);
            await _taskState.UpdateAsync(mission);
        }

        LoadWeekData();
    }
    public async Task UpdateTaskTime(TaskCompletionTime task)
    {
        if (task?.IdMissionNavigation != null)
        {
            await _taskState.UpdateAsync(task.IdMissionNavigation);
            await LoadWeekData(); // Перезагружаем данные
        }
    }
    private void OnTasksChanged(object sender, TaskStateChangedEventArgs e)
    {
        LoadWeekData();
    }
    public Action RefreshTimelinePanels { get; set; }
    //public ICommand ToggleNonWorkingHoursCommand => new RelayCommand(() =>
    //{
    //    foreach (var hour in Hours)
    //    {
    //        if (!hour.IsWorkingHour)
    //        {
    //            hour.IsExpanded = !hour.IsExpanded;
    //        }
    //    }

    //    RefreshTimelinePanels?.Invoke();
    //});
    [ObservableProperty]
    private bool _isNonWorkingHoursExpanded = true;
    public ICommand ToggleNonWorkingHoursCommand => new RelayCommand(() =>
    {
        IsNonWorkingHoursExpanded = !IsNonWorkingHoursExpanded;

        foreach (var hour in Hours)
        {
            if (!hour.IsWorkingHour)
            {
                hour.IsExpanded = IsNonWorkingHoursExpanded;
            }
        }

        RefreshTimelinePanels?.Invoke();
    });
}
