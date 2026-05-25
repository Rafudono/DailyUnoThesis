using System;
using System.Collections.ObjectModel;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;

namespace DailyUnoThesis.Presentation.View.Calendar;

public sealed partial class WeekCalendarView : Page
{
    private WeekCalendarViewModel _viewModel;
    private TaskCompletionTime _draggedTask;
    private TimelinePanel _dragTargetTimeline;

    public WeekCalendarView()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel = this.DataContext as WeekCalendarViewModel;
        SyncScrollViewers();
    }

    private void Task_DragStarting(UIElement sender, DragStartingEventArgs e)
    {
        var border = sender as Border;
        _draggedTask = border?.DataContext as TaskCompletionTime;
        if (_draggedTask == null) return;

        e.Data.Properties.Add("DraggedItem", _draggedTask);
        e.Data.RequestedOperation = DataPackageOperation.Move;
        e.DragUI.SetContentFromDataPackage();

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
        if (_draggedTask == null) return;

        var timeline = sender as TimelinePanel;
        if (timeline == null) return;

        var position = e.GetPosition(timeline);
        var hourHeight = 60.0;
        var hour = position.Y / hourHeight;
        var minute = (position.Y % hourHeight) / hourHeight * 60;
        minute = Math.Round(minute / 15) * 15;

        var targetDate = GetDateFromTimeline(timeline);
        var newStartTime = targetDate.Date.AddHours(hour).AddMinutes(minute);
        var duration = _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value;
        var newEndTime = newStartTime.Add(duration);

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
        e.Handled = true;
    }

    private async void Timeline_Drop(object sender, DragEventArgs e)
    {
        if (_draggedTask == null) return;

        await _viewModel.UpdateTaskTime(_draggedTask);
        _draggedTask = null;

        e.Handled = true;
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

    #region Синхронизация вертикальной прокрутки между всеми днями
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

    private static void SyncScrollView(ScrollViewer target, ScrollViewer source, double offset)
    {
        if (target != null && target != source && Math.Abs(target.VerticalOffset - offset) > 0.5)
            target.ChangeView(null, offset, null);
    }
    #endregion
}
