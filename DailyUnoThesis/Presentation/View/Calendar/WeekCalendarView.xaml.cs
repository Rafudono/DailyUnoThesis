using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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

    public WeekCalendarView()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel = this.DataContext as WeekCalendarViewModel;
       // SyncScrollViewers();
    }


    private void Task_DragStarting(UIElement sender, DragStartingEventArgs e)
    {
        var border = sender as Border;
        _draggedTask = border?.DataContext as TaskCompletionTime;
        if (_draggedTask == null) return;

        e.Data.Properties.Add("DraggedItem", _draggedTask);
        e.Data.RequestedOperation = DataPackageOperation.Move;
        if (_transparentDragImage != null)
            e.DragUI.SetContentFromBitmapImage(_transparentDragImage);

        _dragTargetTimeline = FindParentTimeline(border);
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
        if (_draggedTask == null)
        {
            if (e.DataView.Properties.TryGetValue("DraggedItem", out var item) && item is Mission)
            {
                var missionTimeline = sender as TimelinePanel;
                if (missionTimeline == null) return;

                var missionPos = e.GetPosition(missionTimeline);
                var (missionHour, missionMinute) = GetTimeFromPosition(missionTimeline, missionPos.Y);
                var missionDate = GetDateFromTimeline(missionTimeline);
                var (startTime, endTime, canExecute) = CalculateDragPreview(missionDate, missionHour, missionMinute, item);

                _draggedTask = new TaskCompletionTime
                {
                    IdMissionNavigation = item as Mission,
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
        var duration = _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value;
        var newEndTime = newStartTime.Add(duration);

        if (newEndTime > targetDate.Date.AddDays(1))
        {
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.Handled = true;
            return;
        }

        if (timeline != _dragTargetTimeline)
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
                    var (startTime, endTime, canExecute) = CalculateDragPreview(targetDate, hour, minute, externalTask);
                    if (canExecute)
                    {
                        _draggedTask.StartExecution = startTime;
                        _draggedTask.EndExecution = endTime;
                        await _viewModel.UpdateTaskTime(_draggedTask);
                    }
                    _draggedTask = null;
                    e.Handled = true;
                }
            }
            return;
        }

        // Mission drag — remove temp preview task
        if (_draggedTask.Id == 0 && _draggedTask.IdMissionNavigation is Mission)
        {
            var (fractionalHour, taskDate) = GetPositionFromTask(_draggedTask);
            _viewModel.DropAtTimeSlot(taskDate, fractionalHour, _draggedTask.IdMissionNavigation);
            _draggedTask = null;
            e.Handled = true;
            return;
        }

        await _viewModel.UpdateTaskTime(_draggedTask);
        _draggedTask = null;

        e.Handled = true;
    }

    private (DateTime startTime, DateTime endTime, bool isDoable) CalculateDragPreview(DateTime targetDay, double hour, double minute, object draggedItem)
    {
        var startTime = targetDay.Date.AddHours(hour).AddMinutes(minute);

        if (draggedItem is Mission mission)
        {
            var duration = mission.DurationMinutes > 0
                ? TimeSpan.FromMinutes(mission.DurationMinutes.Value)
                : TimeSpan.FromHours(1);
            var minutsInWorkingDay = (AuthorizedUser.GetInstance().AuthUser.DayEndTime - AuthorizedUser.GetInstance().AuthUser.DayStartTime) * 60;
          return  duration > minutsInWorkingDay
                ? (startTime, startTime.Add(duration),false)
                : (startTime, startTime.Add(duration),true);
        }

        if (draggedItem is TaskCompletionTime task)
        {
            var duration = task.EndExecution.Value - task.StartExecution.Value;
            return (startTime, startTime.Add(duration), true);
        }

        return (startTime, startTime.AddHours(1), true);
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
}

    //#region Синхронизация вертикальной прокрутки между всеми днями

    //private static void SyncScrollView(ScrollViewer target, ScrollViewer source, double offset)
    //{
    //    if (target != null && target != source && Math.Abs(target.VerticalOffset - offset) > 0.5)
    //        target.ChangeView(null, offset, null);
    //}
    //#endregion
/* есть вероятность, что при прокручивании, когда курсор на одном из дней scrollviewrы для каждого дня могут рассинхрониться О_о
     private void SyncScrollViewers()
    {
        MondayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(MondayScrollViewer);
        TuesdayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(TuesdayScrollViewer);
        WednesdayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(WednesdayScrollViewer);
        ThursdayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(ThursdayScrollViewer);
        FridayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(FridayScrollViewer);
        SaturdayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(SaturdayScrollViewer);
        SundayScrollViewer.ViewChanged += (s, e) => SyncAllScrollViews(SundayScrollViewer);
    }

    private void SyncAllScrollViews(ScrollViewer source)
    {
        var offset = source.VerticalOffset;
        SyncScrollView(MondayScrollViewer, source, offset);
        SyncScrollView(TuesdayScrollViewer, source, offset);
        SyncScrollView(WednesdayScrollViewer, source, offset);
        SyncScrollView(ThursdayScrollViewer, source, offset);
        SyncScrollView(FridayScrollViewer, source, offset);
        SyncScrollView(SaturdayScrollViewer, source, offset);
        SyncScrollView(SundayScrollViewer, source, offset);
    } 
 */
