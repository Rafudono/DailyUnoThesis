using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DailyUnoThesis.Presentation.View.Calendar;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class WeekCalendarView : Page
{
    private WeekCalendarViewModel _viewModel;
    private TaskCompletionTime _draggedTask;
    private TimelinePanel _dragTargetTimeline;
    private FrameworkElement _dragPreview;
    public WeekCalendarView()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel = this.DataContext as WeekCalendarViewModel;
        SyncScrollViewers();

        //// Передаем делегат для обновления
        //if (_viewModel != null)
        //{
        //    _viewModel.RefreshTimelinePanels = () =>
        //    {
        //        //MondayTimeline?.InvalidateMeasure();
        //        //MondayTimeline?.InvalidateArrange();
        //        // добавьте остальные дни
        //    };
        //}
    }
    private void Task_DragStarting(UIElement sender, DragStartingEventArgs e)
    {
        var border = sender as Border;
        _draggedTask = border?.DataContext as TaskCompletionTime;
        if (_draggedTask == null) return;

        CleanupDragPreview();

        e.Data.Properties.Add("DraggedItem", _draggedTask);
        e.Data.RequestedOperation = DataPackageOperation.Move;
        e.DragUI.SetContentFromDataPackage();

        _dragTargetTimeline = FindParentTimeline(border);

        // Создаём копию задачи, которая будет двигаться с курсором
        _dragPreview = new Border
        {
            Style = (Style)Resources["TaskItemStyle"],
            Width = border.ActualWidth,
            Height = border.ActualHeight,
            Opacity = 0.85,
            DataContext = _draggedTask,
            Child = new StackPanel
            {
                Margin = new Thickness(4, 2),
                Children =
                {
                    new TextBlock
                    {
                        Text = _draggedTask.IdMissionNavigation.Title,
                        FontSize = 11,
                        FontWeight = FontWeights.SemiBold,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        Foreground = (Brush)Resources["InkBlueLightBrush"]
                    },
                    new TextBlock
                    {
                        Text = _draggedTask.FormattedTime,
                        FontSize = 9,
                        Foreground = (Brush)Resources["OnSurfaceSecondaryBrush"]
                    }
                }
            }
        };

        var parentGrid = VisualTreeHelper.GetParent(_dragTargetTimeline) as Grid;
        if (parentGrid != null)
        {
            parentGrid.Children.Add(_dragPreview);
            _dragPreview.Margin = new Thickness(2, 0, 2, 0);
        }
    }

    private TimelinePanel FindParentTimeline(DependencyObject child)
    {
        while (child != null)
        {
            if (child is TimelinePanel tp) return tp;
            child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }

    private ScrollViewer FindParentScrollViewer(DependencyObject child)
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null)
        {
            if (parent is ScrollViewer sv) return sv;
            parent = VisualTreeHelper.GetParent(parent);
        }
        return null;
    }
    private void Timeline_DragOver(object sender, DragEventArgs e)
    {
        if (_draggedTask == null || _dragPreview == null) return;

        var timeline = sender as TimelinePanel;
        if (timeline == null) return;

        if (timeline != _dragTargetTimeline)
        {
            var oldGrid = VisualTreeHelper.GetParent(_dragTargetTimeline) as Grid;
            var newGrid = VisualTreeHelper.GetParent(timeline) as Grid;
            if (oldGrid != null && newGrid != null)
            {
                oldGrid.Children.Remove(_dragPreview);
                newGrid.Children.Add(_dragPreview);
            }
            _dragTargetTimeline = timeline;
        }

        var position = e.GetPosition(timeline);
        var hourHeight = 60.0;
        var hour = position.Y / hourHeight;
        var minute = (position.Y % hourHeight) / hourHeight * 60;
        minute = Math.Floor(minute / 15) * 15;

        var targetDate = GetDateFromTimeline(timeline);
        var newStartTime = targetDate.Date.AddHours(hour).AddMinutes(minute);
        var duration = _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value;
        var newEndTime = newStartTime.Add(duration);

        double top = timeline.GetTopPosition(newStartTime);
        _dragPreview.Margin = new Thickness(2, top, 2, 0);

        if (_dragPreview is Border previewBorder && previewBorder.Child is StackPanel sp && sp.Children[1] is TextBlock tb)
        {
            tb.Text = $"{newStartTime:HH:mm} - {newEndTime:HH:mm}";
        }

        e.AcceptedOperation = DataPackageOperation.Move;
        e.Handled = true;
    }
    private async void Timeline_Drop(object sender, DragEventArgs e)
    {
        if (_draggedTask == null) return;

        var timeline = sender as TimelinePanel;
        if (timeline == null) return;

        if (timeline != _dragTargetTimeline)
        {
            var oldGrid = VisualTreeHelper.GetParent(_dragTargetTimeline) as Grid;
            var newGrid = VisualTreeHelper.GetParent(timeline) as Grid;
            if (oldGrid != null && newGrid != null)
            {
                oldGrid.Children.Remove(_dragPreview);
                newGrid.Children.Add(_dragPreview);
            }
            _dragTargetTimeline = timeline;
        }

        var position = e.GetPosition(timeline);
        var hourHeight = 60.0;
        var hour = position.Y / hourHeight;
        var minute = (position.Y % hourHeight) / hourHeight * 60;
        minute = Math.Floor(minute / 15) * 15;

        var targetDate = GetDateFromTimeline(timeline);
        var newStartTime = targetDate.Date.AddHours(hour).AddMinutes(minute);
        var duration = _draggedTask.EndExecution.Value - _draggedTask.StartExecution.Value;
        _draggedTask.StartExecution = newStartTime;
        _draggedTask.EndExecution = newStartTime.Add(duration);

        await _viewModel.UpdateTaskTime(_draggedTask);
        _draggedTask = null;

        CleanupDragPreview();

        e.Handled = true;
    }

    private void CleanupDragPreview()
    {
        if (_dragPreview != null)
        {
            var parent = VisualTreeHelper.GetParent(_dragPreview) as Grid;
            parent?.Children.Remove(_dragPreview);
            _dragPreview = null;
        }
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
    public async Task UpdateTaskTime(TaskCompletionTime task)
    {
        if (task?.IdMissionNavigation != null)
        {
            //await _taskState.UpdateAsync(task.IdMissionNavigation);
            //await LoadWeekData();
        }
    }

    #region Синхронизация вертикальной прокрутки между всеми днями
    private void SyncScrollViewers()
    {
        // Подписываемся на события прокрутки каждого дня
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

    private void SyncScrollView(ScrollViewer target, ScrollViewer source, double offset)
    {
        if (target != null && target != source && Math.Abs(target.VerticalOffset - offset) > 0.5)
        {
            target.ChangeView(null, offset, null);
        }
    }
    #endregion
}
