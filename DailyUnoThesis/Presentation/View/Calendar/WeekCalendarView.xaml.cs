using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace DailyUnoThesis.Presentation.View.Calendar;

public sealed partial class WeekCalendarView : Page
{
    private WeekCalendarViewModel _viewModel;
    private TaskCompletionTime _draggedTask;
    private TimelinePanel _dragTargetTimeline;
    private BitmapImage _transparentDragImage;
    private DateTime _dragTargetTimelessDate;
    private bool _wasZeroDuration;

    public WeekCalendarView()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel = this.DataContext as WeekCalendarViewModel;
        _transparentDragImage = new BitmapImage(new Uri("ms-appx:///Assets/TransparentDrag.png"));
       // SyncScrollViewers();
    }

    private void WeekView_DragLeave(object sender, DragEventArgs e)
    {
        ClearLocalHint();

        if (_draggedTask == null || _draggedTask.Id != 0) return;

        if (_dragTargetTimeline != null)
        {
            GetSessionCollection(_dragTargetTimeline)?.Remove(_draggedTask);
            _dragTargetTimeline = null;
        }

        if (_dragTargetTimelessDate != default)
        {
            GetTimelessCollection(_dragTargetTimelessDate)?.Remove(_draggedTask);
            _dragTargetTimelessDate = default;
        }

        _draggedTask = null;
        _wasZeroDuration = false;
    }

    private void SetLocalHint(string? message = null)
    {
        DragHint.Text = message ?? "перенесите задачу на календарь";
    }

    private void ClearLocalHint()
    {
        DragHint.Text = string.Empty;
    }

    private void Task_DragStarting(UIElement sender, DragStartingEventArgs e)
    {
        var border = sender as Border;
        _draggedTask = border?.DataContext as TaskCompletionTime;
        if (_draggedTask == null) return;

        e.Data.Properties.Add("DraggedItem", _draggedTask);
        DeadlineHelper.AddDeadlineProperties(e.Data.Properties, _draggedTask.IdMissionNavigation);
        e.Data.RequestedOperation = DataPackageOperation.Move;
        if (_transparentDragImage != null)
            e.DragUI.SetContentFromBitmapImage(_transparentDragImage);

        _dragTargetTimeline = FindParentTimeline(border);
        _wasZeroDuration = _draggedTask.EndExecution.HasValue && _draggedTask.StartExecution.HasValue &&
            _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value <= TimeSpan.Zero;
    }

    private void TimelessTask_DragStarting(UIElement sender, DragStartingEventArgs e)
    {
        var border = sender as Border;
        _draggedTask = border?.DataContext as TaskCompletionTime;
        if (_draggedTask == null) return;

        e.Data.Properties.Add("DraggedItem", _draggedTask);
        DeadlineHelper.AddDeadlineProperties(e.Data.Properties, _draggedTask.IdMissionNavigation);
        e.Data.RequestedOperation = DataPackageOperation.Move;
        if (_transparentDragImage != null)
            e.DragUI.SetContentFromBitmapImage(_transparentDragImage);

        _dragTargetTimeline = null;
        _dragTargetTimelessDate = default;
        _wasZeroDuration = _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value <= TimeSpan.Zero;

        GetTimelessCollection(_draggedTask.StartExecution?.Date ?? default)?.Remove(_draggedTask);
    }

    private static TimelinePanel FindParentTimeline(DependencyObject child)
    {
        while (child != null)
        {
            if (child is TimelinePanel tp) return tp;
            child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }

    private void Timeline_DragOver(object sender, DragEventArgs e)
    {
        SetLocalHint();

        if (_draggedTask == null)
        {
            if (e.DataView.Properties.TryGetValue("DraggedItem", out var item) && item is Mission mission)
            {
                var missionTimeline = sender as TimelinePanel;
                if (missionTimeline == null) return;

                var missionDate = GetDateFromTimeline(missionTimeline);

                if (!DeadlineHelper.IsDateAllowedByDeadline(missionDate.Date, e.DataView.Properties))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    e.DragUIOverride.IsCaptionVisible = false;
                    e.DragUIOverride.IsGlyphVisible = false;
                    e.Handled = true;
                    SetLocalHint("вы вышли за пределы дедлайна");
                    return;
                }

                var missionPos = e.GetPosition(missionTimeline);
                var (missionHour, missionMinute) = GetTimeFromPosition(missionTimeline, missionPos.Y);
                var (startTime, endTime) = CalculateDragPreview(missionDate, missionHour, missionMinute, mission);

                if (!_viewModel.IsNonWorkingHoursExpanded)
                {
                    var user = AuthorizedUser.GetInstance().AuthUser;
                    if ((user.DayStartTime != null && startTime < missionDate.Date.AddHours(user.DayStartTime.Value.Hour)) ||
                        (user.DayEndTime != null && endTime > missionDate.Date.AddHours(user.DayEndTime.Value.Hour)))
                    {
                        e.AcceptedOperation = DataPackageOperation.None;
                        e.DragUIOverride.IsCaptionVisible = false;
                        e.DragUIOverride.IsGlyphVisible = false;
                        e.Handled = true;
                        return;
                    }
                }

                if (!DeadlineHelper.IsAllowedByDeadlineTime(endTime, e.DataView.Properties))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    e.DragUIOverride.IsCaptionVisible = false;
                    e.DragUIOverride.IsGlyphVisible = false;
                    e.Handled = true;
                    SetLocalHint("вы вышли за пределы дедлайна");
                    return;
                }

                _draggedTask = new TaskCompletionTime
                {
                    IdMissionNavigation = mission,
                    StartExecution = startTime,
                    EndExecution = endTime
                };
                _dragTargetTimeline = missionTimeline;

                var targetCollection = GetSessionCollection(missionTimeline);
                targetCollection?.Add(_draggedTask);

                missionTimeline.InvalidateArrange();
                missionTimeline.InvalidateMeasure();

                e.AcceptedOperation = DataPackageOperation.Move;
                e.DragUIOverride.IsCaptionVisible = false;
                e.DragUIOverride.IsGlyphVisible = false;
                e.Handled = true;
                return;
            }
            return;
        }

        var timeline = sender as TimelinePanel;
        if (timeline == null) return;

        var position = e.GetPosition(timeline);
        var (hour, minute) = GetTimeFromPosition(timeline, position.Y);

        var targetDate = GetDateFromTimeline(timeline);
        var newStartTime = targetDate.Date.AddHours(hour).AddMinutes(minute);

        if (!DeadlineHelper.IsDateAllowedByDeadline(targetDate.Date, e.DataView.Properties))
        {
            e.AcceptedOperation = DataPackageOperation.None;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.Handled = true;
            SetLocalHint("вы вышли за пределы дедлайна");
            return;
        }

        if (_wasZeroDuration)
        {
            if (_dragTargetTimeline == null)
            {
                GetSessionCollection(timeline)?.Add(_draggedTask);
                _dragTargetTimeline = timeline;
            }
            else if (timeline != _dragTargetTimeline)
            {
                var sourceCollection = GetSessionCollection(_dragTargetTimeline);
                var targetCollection = GetSessionCollection(timeline);
                sourceCollection?.Remove(_draggedTask);
                targetCollection?.Add(_draggedTask);
                _dragTargetTimeline = timeline;
            }

            _draggedTask.StartExecution = newStartTime;
            _draggedTask.EndExecution = newStartTime;

            if (!_viewModel.IsNonWorkingHoursExpanded)
            {
                var user = AuthorizedUser.GetInstance().AuthUser;
                if ((user.DayStartTime != null && newStartTime < targetDate.Date.AddHours(user.DayStartTime.Value.Hour)) ||
                    (user.DayEndTime != null && newStartTime > targetDate.Date.AddHours(user.DayEndTime.Value.Hour)))
                {
                    e.DragUIOverride.IsCaptionVisible = false;
                    e.DragUIOverride.IsGlyphVisible = false;
                    e.Handled = true;
                    return;
                }
            }

            if (!DeadlineHelper.IsAllowedByDeadlineTime(newStartTime, e.DataView.Properties))
            {
                e.DragUIOverride.IsCaptionVisible = false;
                e.DragUIOverride.IsGlyphVisible = false;
                e.Handled = true;
                SetLocalHint("вы вышли за пределы дедлайна");
                return;
            }

            timeline.InvalidateArrange();
            timeline.InvalidateMeasure();
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.Handled = true;
            return;
        }

        var duration = _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value;
        var newEndTime = newStartTime.Add(duration);

        if (newEndTime > targetDate.Date.AddDays(1))
        {
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.Handled = true;
            return;
        }

        if (!_viewModel.IsNonWorkingHoursExpanded)
        {
            var user = AuthorizedUser.GetInstance().AuthUser;
            if ((user.DayStartTime != null && newStartTime < targetDate.Date.AddHours(user.DayStartTime.Value.Hour)) ||
                (user.DayEndTime != null && newEndTime > targetDate.Date.AddHours(user.DayEndTime.Value.Hour)))
            {
                e.DragUIOverride.IsCaptionVisible = false;
                e.DragUIOverride.IsGlyphVisible = false;
                e.Handled = true;
                return;
            }
        }

        if (!DeadlineHelper.IsAllowedByDeadlineTime(newEndTime, e.DataView.Properties))
        {
            e.AcceptedOperation = DataPackageOperation.None;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.Handled = true;
            SetLocalHint("вы вышли за пределы дедлайна");
            return;
        }

        if (_dragTargetTimeline == null)
        {
            GetSessionCollection(timeline)?.Add(_draggedTask);
            _dragTargetTimeline = timeline;
        }
        else if (timeline != _dragTargetTimeline)
        {
            var sourceCollection = GetSessionCollection(_dragTargetTimeline);
            var targetCollection = GetSessionCollection(timeline);
            sourceCollection?.Remove(_draggedTask);
            targetCollection?.Add(_draggedTask);
            _dragTargetTimeline = timeline;
        }

        _draggedTask.StartExecution = newStartTime;
        _draggedTask.EndExecution = newEndTime;

        timeline.InvalidateArrange();
        timeline.InvalidateMeasure();

        e.AcceptedOperation = DataPackageOperation.Move;
        e.DragUIOverride.IsCaptionVisible = false;
        e.DragUIOverride.IsGlyphVisible = false;
        e.Handled = true;
    }

    private async void Timeline_Drop(object sender, DragEventArgs e)
    {
        ClearLocalHint();

        if (_draggedTask == null)
        {
            if (e.DataView.Properties.TryGetValue("DraggedItem", out var item))
            {
                var timeline = sender as TimelinePanel;
                if (timeline == null) return;

                var position = e.GetPosition(timeline);
                var (hour, minute) = GetTimeFromPosition(timeline, position.Y);
                var targetDate = GetDateFromTimeline(timeline);

                if (item is Mission mission)
                {
                    _viewModel.DropAtTimeSlot(targetDate, hour + minute / 60.0, mission);
                    e.Handled = true;
                }
                else if (item is TaskCompletionTime externalTask)
                {
                    _draggedTask = externalTask;
                    _dragTargetTimeline = timeline;
                    var (startTime, endTime) = CalculateDragPreview(targetDate, hour, minute, externalTask);
                    _draggedTask.StartExecution = startTime;
                    _draggedTask.EndExecution = endTime;
                    await _viewModel.UpdateTaskTime(_draggedTask);
                    _draggedTask = null;
                    e.Handled = true;
                }
            }
            return;
        }

        // Mission drag — remove temp preview task
        if (_draggedTask.Id == 0 && _draggedTask.IdMissionNavigation is Mission ghostMission)
        {
            var dropTimeline = sender as TimelinePanel;
            if (dropTimeline != null)
            {
                var dropDate = GetDateFromTimeline(dropTimeline);
                if (!DeadlineHelper.IsDateAllowedByDeadline(dropDate.Date, e.DataView.Properties))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    e.Handled = true;
                    SetLocalHint("вы вышли за пределы дедлайна");
                    return;
                }
            }

            var startTime = _draggedTask.StartExecution.Value;
            var newSession = new TaskCompletionTime
            {
                IdMission = ghostMission.Id,
                StartExecution = startTime,
                EndExecution = startTime,
                IdMissionNavigation = ghostMission
            };
            ghostMission.TaskCompletionTimes ??= new List<TaskCompletionTime>();
            ghostMission.TaskCompletionTimes.Add(newSession);
            await _viewModel.UpdateTaskTime(newSession);
            _draggedTask = null;
            _wasZeroDuration = false;
            e.Handled = true;
            return;
        }

        if (_wasZeroDuration)
        {
            _draggedTask.EndExecution = _draggedTask.StartExecution;
            _wasZeroDuration = false;
        }
        await _viewModel.UpdateTaskTime(_draggedTask);
        _draggedTask = null;

        e.Handled = true;
        ClearLocalHint();
    }

    private (DateTime startTime, DateTime endTime) CalculateDragPreview(DateTime targetDay, double hour, double minute, object draggedItem)
    {
        var startTime = targetDay.Date.AddHours(hour).AddMinutes(minute);

        if (draggedItem is Mission mission)
        {
            var duration = mission.DurationMinutes > 0
                ? TimeSpan.FromMinutes(mission.DurationMinutes.Value)
                : TimeSpan.FromHours(1);
            return (startTime, startTime.Add(duration));
        }

        if (draggedItem is TaskCompletionTime task)
        {
            var duration = task.EndExecution.Value - task.StartExecution.Value;
            return (startTime, startTime.Add(duration));
        }

        return (startTime, startTime.AddHours(1));
    }

    private (double hour, DateTime targetDate) GetPositionFromTask(TaskCompletionTime task)
    {
        var start = task.StartExecution.Value;
        return (start.Hour + start.Minute / 60.0, start.Date);
    }

    private ObservableCollection<TaskCompletionTime> GetSessionCollection(TimelinePanel timeline)
    {
        if (timeline == MondayTimeline) return _viewModel.MondaySessions;
        if (timeline == TuesdayTimeline) return _viewModel.TuesdaySessions;
        if (timeline == WednesdayTimeline) return _viewModel.WednesdaySessions;
        if (timeline == ThursdayTimeline) return _viewModel.ThursdaySessions;
        if (timeline == FridayTimeline) return _viewModel.FridaySessions;
        if (timeline == SaturdayTimeline) return _viewModel.SaturdaySessions;
        if (timeline == SundayTimeline) return _viewModel.SundaySessions;
        return null;
    }

    private DateTime GetDateFromTimeline(TimelinePanel timeline)
    {
        if (timeline == MondayTimeline) return _viewModel.CurrentWeekStart.AddDays(0);
        if (timeline == TuesdayTimeline) return _viewModel.CurrentWeekStart.AddDays(1);
        if (timeline == WednesdayTimeline) return _viewModel.CurrentWeekStart.AddDays(2);
        if (timeline == ThursdayTimeline) return _viewModel.CurrentWeekStart.AddDays(3);
        if (timeline == FridayTimeline) return _viewModel.CurrentWeekStart.AddDays(4);
        if (timeline == SaturdayTimeline) return _viewModel.CurrentWeekStart.AddDays(5);
        if (timeline == SundayTimeline) return _viewModel.CurrentWeekStart.AddDays(6);
        return DateTime.Today;
    }


    private ObservableCollection<TaskCompletionTime> GetTimelessCollection(DateTime date)
    {
        if (date.Date == _viewModel.CurrentWeekStart) return _viewModel.MondayTimelessSessions;
        if (date.Date == _viewModel.CurrentWeekStart.AddDays(1)) return _viewModel.TuesdayTimelessSessions;
        if (date.Date == _viewModel.CurrentWeekStart.AddDays(2)) return _viewModel.WednesdayTimelessSessions;
        if (date.Date == _viewModel.CurrentWeekStart.AddDays(3)) return _viewModel.ThursdayTimelessSessions;
        if (date.Date == _viewModel.CurrentWeekStart.AddDays(4)) return _viewModel.FridayTimelessSessions;
        if (date.Date == _viewModel.CurrentWeekStart.AddDays(5)) return _viewModel.SaturdayTimelessSessions;
        if (date.Date == _viewModel.CurrentWeekStart.AddDays(6)) return _viewModel.SundayTimelessSessions;
        return null;
    }

    private (int hour, double minute) GetTimeFromPosition(TimelinePanel timeline, double y)
    {
        if (timeline.Hours == null)
        {
            var h = (int)(y / timeline.HourHeight);
            var m = (y % timeline.HourHeight) / timeline.HourHeight * 60;
            m = Math.Round(m / 15) * 15;
            if (m >= 60)
                return (h + 1, 0);
            return (h, Math.Max(0, m));
        }

        double accumulated = 0;
        HourSlot lastSlot = null;
        foreach (HourSlot slot in timeline.Hours)
        {
            lastSlot = slot;
            double slotHeight = slot.Height;
            if (y >= accumulated && y < accumulated + slotHeight)
            {
                if (slotHeight <= 2)
                    return (slot.Hour, 0);
                double offsetInSlot = y - accumulated;
                double minute = (offsetInSlot / slotHeight) * 60;
                minute = Math.Round(minute / 15) * 15;
                if (minute >= 60)
                    return (slot.Hour + 1, 0);
                return (slot.Hour, Math.Max(0, minute));
            }
            accumulated += slotHeight;
        }

        return lastSlot != null ? (lastSlot.Hour, 0) : (0, 0);
    }


    private TaskCompletionTime _resizingTask;
    private bool _isResizingTop;
    private TimelinePanel _resizeTimeline;
    private double _resizeStartY;
    private DateTime _resizeOriginalStart;
    private DateTime _resizeOriginalEnd;

    private void ResizeGrip_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var grip = sender as FrameworkElement;
        _resizingTask = grip?.DataContext as TaskCompletionTime;
        if (_resizingTask == null || !_resizingTask.StartExecution.HasValue || !_resizingTask.EndExecution.HasValue) return;

        _isResizingTop = grip.VerticalAlignment == VerticalAlignment.Top;
        _resizeTimeline = FindParentTimeline(grip);
        if (_resizeTimeline == null) return;

        _resizeOriginalStart = _resizingTask.StartExecution.Value;
        _resizeOriginalEnd = _resizingTask.EndExecution.Value;
        _resizeStartY = e.GetCurrentPoint(_resizeTimeline).Position.Y;

        grip.CapturePointer(e.Pointer);
        e.Handled = true;
    }

    private void ResizeGrip_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_resizingTask == null || _resizeTimeline == null) return;

        var currentY = e.GetCurrentPoint(_resizeTimeline).Position.Y;
        var deltaY = currentY - _resizeStartY;
        var deltaMinutes = (deltaY / _resizeTimeline.HourHeight) * 60;
        deltaMinutes = Math.Round(deltaMinutes / 15) * 15;

        if (_isResizingTop)
        {
            var newStart = _resizeOriginalStart.AddMinutes(deltaMinutes);
            if (newStart < _resizingTask.EndExecution.Value && newStart.Date == _resizeOriginalStart.Date)
            {
                _resizingTask.StartExecution = newStart;
            }
        }
        else
        {
            var newEnd = _resizeOriginalEnd.AddMinutes(deltaMinutes);
            if (newEnd > _resizingTask.StartExecution.Value && newEnd.Date == _resizeOriginalEnd.Date)
            {
                _resizingTask.EndExecution = newEnd;
            }
        }

        _resizeTimeline.InvalidateArrange();
        e.Handled = true;
    }

    private void ResizeGrip_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border grip)
            grip.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(120, 139, 92, 246));
    }

    private void ResizeGrip_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border grip)
            grip.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(21, 139, 92, 246));
    }

    private async void ResizeGrip_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_resizingTask == null) return;

        var grip = sender as FrameworkElement;
        grip?.ReleasePointerCapture(e.Pointer);

        if (_resizingTask.IdMissionNavigation != null)
        {
            await _viewModel.UpdateTaskTime(_resizingTask);
        }

        _resizingTask = null;
        _resizeTimeline = null;
        e.Handled = true;
    }

    private void OpenEditTaskFrame(object sender, DoubleTappedRoutedEventArgs e)
    {
        var border = sender as Border;
        if (border == null) return;
        var session = border.DataContext as TaskCompletionTime;
        if (session?.IdMissionNavigation != null)
        {
            ViewModelStore.GetInstance().CalendarViewModel?.OpenTaskEditor(session.IdMissionNavigation);
        }
    }

    private DateTime GetDateFromListView(FrameworkElement element)
    {
        var parent = VisualTreeHelper.GetParent(element);
        while (parent != null && !(parent is Border))
            parent = VisualTreeHelper.GetParent(parent);
        if (parent is Border border)
        {
            var column = Grid.GetColumn(border);
            return _viewModel.CurrentWeekStart.AddDays(column - 1);
        }
        return DateTime.Today;
    }

    private async void TimelessList_Drop(object sender, DragEventArgs e)
    {
        ClearLocalHint();

        var itemsControl = sender as ItemsControl;
        if (itemsControl == null) return;

        var targetDate = GetDateFromListView(itemsControl);

        if (_draggedTask != null)
        {
            if (_draggedTask.Id == 0 && _draggedTask.IdMissionNavigation is Mission ghostMission)
            {
                if (!DeadlineHelper.IsDateAllowedByDeadline(targetDate.Date, e.DataView.Properties))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    e.Handled = true;
                    SetLocalHint("вы вышли за пределы дедлайна");
                    return;
                }

                var newSession = new TaskCompletionTime
                {
                    IdMission = ghostMission.Id,
                    StartExecution = targetDate.Date.AddSeconds(1),
                    EndExecution = targetDate.Date.AddSeconds(1),
                    IdMissionNavigation = ghostMission
                };
                ghostMission.TaskCompletionTimes ??= new List<TaskCompletionTime>();
                ghostMission.TaskCompletionTimes.Add(newSession);
                await _viewModel.UpdateTaskTime(newSession);
            }
            else
            {
                _draggedTask.StartExecution = targetDate.Date.AddSeconds(1);
                _draggedTask.EndExecution = targetDate.Date.AddSeconds(1);
                await _viewModel.UpdateTaskTime(_draggedTask);
            }
            _draggedTask = null;
            e.Handled = true;
            ClearLocalHint();
            return;
        }

        if (e.DataView.Properties.TryGetValue("DraggedItem", out var item))
        {
            if (item is TaskCompletionTime t)
            {
                t.StartExecution = targetDate.Date.AddSeconds(1);
                t.EndExecution = targetDate.Date.AddSeconds(1);
                await _viewModel.UpdateTaskTime(t);
                e.Handled = true;
            }
            else if (item is Mission mission)
            {
                if (!DeadlineHelper.IsDateAllowedByDeadline(targetDate.Date, e.DataView.Properties))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    e.Handled = true;
                    SetLocalHint("вы вышли за пределы дедлайна");
                    return;
                }

                var newSession = new TaskCompletionTime
                {
                    IdMission = mission.Id,
                    StartExecution = targetDate.Date.AddSeconds(1),
                    EndExecution = targetDate.Date.AddSeconds(1),
                    IdMissionNavigation = mission
                };
                mission.TaskCompletionTimes ??= new List<TaskCompletionTime>();
                mission.TaskCompletionTimes.Add(newSession);
                await _viewModel.UpdateTaskTime(newSession);
                e.Handled = true;
            }
        }
    }

    private void TimelessList_DragOver(object sender, DragEventArgs e)
    {
        var itemsControl = sender as ItemsControl;
        if (itemsControl == null) return;

        var targetDate = GetDateFromListView(itemsControl);

        if (_draggedTask == null)
        {
            if (e.DataView.Properties.TryGetValue("DraggedItem", out var item) && item is Mission mission)
            {
                if (!DeadlineHelper.IsDateAllowedByDeadline(targetDate.Date, e.DataView.Properties))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    e.DragUIOverride.IsCaptionVisible = false;
                    e.DragUIOverride.IsGlyphVisible = false;
                    e.Handled = true;
                    SetLocalHint("вы вышли за пределы дедлайна");
                    return;
                }

                _draggedTask = new TaskCompletionTime
                {
                    IdMissionNavigation = mission,
                    StartExecution = targetDate.Date.AddSeconds(1),
                    EndExecution = targetDate.Date.AddSeconds(1)
                };
                GetTimelessCollection(targetDate)?.Add(_draggedTask);
                _dragTargetTimelessDate = targetDate;
            }
        }
        else
        {
            if (!DeadlineHelper.IsDateAllowedByDeadline(targetDate.Date, e.DataView.Properties))
            {
                e.AcceptedOperation = DataPackageOperation.None;
                e.DragUIOverride.IsCaptionVisible = false;
                e.DragUIOverride.IsGlyphVisible = false;
                e.Handled = true;
                SetLocalHint("вы вышли за пределы дедлайна");
                return;
            }

            if (_dragTargetTimelessDate != targetDate)
            {
                if (_dragTargetTimelessDate != default)
                    GetTimelessCollection(_dragTargetTimelessDate)?.Remove(_draggedTask);
                GetTimelessCollection(targetDate)?.Add(_draggedTask);
                _dragTargetTimelessDate = targetDate;
            }
            _draggedTask.StartExecution = targetDate.Date.AddSeconds(1);
            _draggedTask.EndExecution = targetDate.Date.AddSeconds(1);
        }

        e.AcceptedOperation = DataPackageOperation.Move;
        e.DragUIOverride.IsCaptionVisible = false;
        e.DragUIOverride.IsGlyphVisible = false;
        e.Handled = true;
    }

}
