using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DailyUnoThesis.Models.Conventers;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
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
public sealed partial class MonthCalendarView : Page
{
    private Grid _highlightedDayGrid;
    private bool _isNewTaskFrameInitialized;
    public MonthCalendarView()
    {
        this.InitializeComponent();
    }
    private void StackPanel_DragStarting(object sender, DragStartingEventArgs e)
    {
        var stackPanel = sender as StackPanel;

        var session = stackPanel?.DataContext as TaskCompletionTime;
        if (session != null)
        {
            e.Data.Properties.Add("DraggedItem", session);
            e.Data.RequestedOperation = DataPackageOperation.Move;
            return;
        }

        var mission = stackPanel?.DataContext as Mission;
        if (mission != null)
        {
            e.Data.Properties.Add("DraggedItem", mission);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
    }
    private void PlannedListView_DragOver(object sender, DragEventArgs e)
    {
        var viewModel = this.DataContext as MonthCalendarViewModel;
        if (viewModel == null) return;

        e.AcceptedOperation = DataPackageOperation.Move;
        e.DragUIOverride.IsCaptionVisible = false;
        e.DragUIOverride.IsGlyphVisible = false;

        var dayGrid = sender as Grid;
        if (dayGrid == null) return;
        var listView = FindListViewChild(dayGrid);
        if (listView == null) return;

        var position = e.GetPosition(listView);

        if (dayGrid.DataContext is not CalendarDay targetDay) return;

        // Сначала сбрасываем фон у всех дней
        ResetAllDaysBackground();
        SetLocalHint();

        // Нельзя вставлять в другой месяц
        if (targetDay.IsOtherMonth)
        {
            _highlightedDayGrid = dayGrid;
            dayGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 25, 25, 112));
            viewModel.OnDragOver(-1);
            e.AcceptedOperation = DataPackageOperation.None;
            SetLocalHint("вы вышли за пределы месяца");
            return;
        }

        if (!e.DataView.Properties.TryGetValue("DraggedItem", out object draggedItem)) return;

        // Проверяем дедлайн от родительской миссии
        if (draggedItem is Mission draggedMission && !IsDateAllowedByDeadline(targetDay.Date.Date, e.DataView.Properties))
        {
            if (dayGrid != null)
            {
                _highlightedDayGrid = dayGrid;
                dayGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 25, 25, 112));
            }
            viewModel.OnDragOver(-1);
            e.AcceptedOperation = DataPackageOperation.None;
            SetLocalHint("вы вышли за пределы дедлайна");
            return;
        }
        // var draggedSession = draggedItem as TaskCompletionTime;
        // if (draggedSession == null) return;

        // Пустой список
        if (listView.Items.Count == 0)
        {
            ResetAllMargins(listView);
            bool canInsert = false;
            if (draggedItem is TaskCompletionTime draggedSession)
                canInsert = viewModel.CanInsertSessionAt(targetDay, 0, draggedSession);
            else if (draggedItem is Mission mission)
                canInsert = viewModel.CanInsertMissionAt(targetDay, 0, mission);

            viewModel.OnDragOver(canInsert ? 0 : -1);
            return;
        }

        // Непустой список
        for (int i = 0; i < listView.Items.Count; i++)
        {
            var container = listView.ContainerFromIndex(i) as ListViewItem;
            if (container != null)
            {
                var bounds = container.TransformToVisual(listView)
                    .TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

                var border = FindChild<Border>(container, "RootBorder");
                if (border != null)
                {
                    border.Margin = new Thickness(0);
                    border.BorderThickness = new Thickness(0);
                    border.ClearValue(Border.BorderBrushProperty);

                    if (position.Y >= bounds.Top && position.Y <= bounds.Bottom)
                    {
                        int insertIndex = position.Y < bounds.Top + bounds.Height / 2 ? i : i + 1;
                        bool isInsertBefore = insertIndex > i;
                        bool canInsert = false;
                        if (draggedItem is TaskCompletionTime draggedSession)
                            canInsert = viewModel.CanInsertSessionAt(targetDay, insertIndex, draggedSession);
                        else if (draggedItem is Mission mission)
                            canInsert = viewModel.CanInsertMissionAt(targetDay, insertIndex, mission);

                        if (canInsert)
                        {
                            border.Margin = isInsertBefore ? new Thickness(0, 0, 0, 20) : new Thickness(0, 20, 0, 0);
                            border.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 75, 0, 130));
                            border.BorderThickness = new Thickness(0, 2, 0, 0);
                            viewModel.OnDragOver(insertIndex);
                        }
                        else
                        {
                            border.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 100, 20));
                            border.BorderThickness = new Thickness(2);
                            viewModel.OnDragOver(-1);
                        }
                        return;
                    }
                }
            }
        }

        // Конец списка
        int lastIndex = listView.Items.Count;
        bool canInsertAtEnd = false;

        if (draggedItem is TaskCompletionTime session)
            canInsertAtEnd = viewModel.CanInsertSessionAt(targetDay, lastIndex, session);
        else if (draggedItem is Mission mission)
            canInsertAtEnd = viewModel.CanInsertMissionAt(targetDay, lastIndex, mission);

        viewModel.OnDragOver(canInsertAtEnd ? lastIndex : -1);
    }
    //private void ResetAllDaysBackground()
    //{
    //    if (CalendarDaysGrid == null) return;

    //    for (int i = 0; i < CalendarDaysGrid.Items.Count; i++)
    //    {
    //        var container = CalendarDaysGrid.ContainerFromIndex(i) as FrameworkElement;
    //        if (container != null)
    //        {
    //            // Сбрасываем фон на оригинальный
    //            var day = CalendarDaysGrid.Items[i] as CalendarDay;
    //            if (day != null)
    //            {
    //                var converter = new BoolToColorConverter();
    //             var parameter = day.IsOtherMonth ? "LightGray" : "LightBlue";
    //            container.SetValue(Grid.BackgroundProperty,
    //                converter.Convert(day.IsOtherMonth, typeof(Brush), parameter, null));
    //            }
    //        }
    //    }
    //}
    private void ResetAllDaysBackground()
    {
        if (CalendarDaysGrid == null) return;

        for (int i = 0; i < CalendarDaysGrid.Items.Count; i++)
        {
            var container = CalendarDaysGrid.ContainerFromIndex(i) as FrameworkElement;
            if (container != null)
            {
                var day = CalendarDaysGrid.Items[i] as CalendarDay;
                if (day != null)
                {
                    var converter = new BoolToColorConverter();
                    var bgColor = day.IsOtherMonth ? "LightGray" : "LightBlue";

                    container.SetValue(Grid.BackgroundProperty,
                        converter.Convert(day.IsOtherMonth, typeof(Brush), bgColor, null));

                    container.SetValue(FrameworkElement.MarginProperty, new Thickness(2));

                    if (container is Grid grid)
                    {
                        grid.CornerRadius = new CornerRadius(4);
                    }
                    if (_highlightedDayGrid == container)
                    {
                        _highlightedDayGrid = null;
                    }
                }
            }
        }
    }
    private T FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T element && element.Name == name)
                return element;

            var result = FindChild<T>(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private static ListView FindListViewChild(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is ListView lv) return lv;
            var result = FindListViewChild(child);
            if (result != null) return result;
        }
        return null;
    }


    private void PlannedListView_DragLeave(object sender, DragEventArgs e)
    {
        var dayGrid = sender as Grid;
        if (dayGrid == null) return;
        var listView = FindListViewChild(dayGrid);
        if (listView == null) return;

        ResetAllMargins(listView);

        // Сбрасываем подсветку дня
        if (_highlightedDayGrid != null)
        {
            _highlightedDayGrid.ClearValue(Grid.BackgroundProperty);
            _highlightedDayGrid = null;
        }
        ResetAllDaysBackground();
        var viewModel = this.DataContext as MonthCalendarViewModel;
        viewModel?.OnDragOver(-1);
        ClearLocalHint();
    }
    //private void ResetAllMargins(ListView listView)
    //{
    //    for (int i = 0; i < listView.Items.Count; i++)
    //    {
    //        var container = listView.ContainerFromIndex(i) as ListViewItem;
    //        if (container != null)
    //        {
    //            var border = FindChild<Border>(container, "RootBorder");
    //            if (border != null)
    //            {
    //                border.Margin = new Thickness(0);
    //                border.BorderThickness = new Thickness(0);
    //                border.ClearValue(Border.BorderBrushProperty);
    //            }
    //        }
    //    }
    //}
    private void ResetAllMargins(ListView listView)
    {
        for (int i = 0; i < listView.Items.Count; i++)
        {
            var container = listView.ContainerFromIndex(i) as ListViewItem;
            if (container != null)
            {
                var border = FindChild<Border>(container, "RootBorder");
                if (border != null)
                {
                    // Исходный Margin как в стиле CalendarMissionBorderStyle (0,2,0,2)
                    border.Margin = new Thickness(0, 2, 0, 2);
                    border.BorderThickness = new Thickness(0);
                    border.ClearValue(Border.BorderBrushProperty);
                }
            }
        }
    }
    private async void PlannedListView_Drop(object sender, DragEventArgs e)
    {
        var viewModel = this.DataContext as MonthCalendarViewModel;
        if (viewModel == null) return;

        var dayGrid = sender as Grid;
        if (dayGrid == null) return;
        var listView = FindListViewChild(dayGrid);
        if (listView == null) return;

        int insertIndex = viewModel.DropTargetIndex;
        if (insertIndex < 0) return;

        if (dayGrid.DataContext is not CalendarDay targetDay) return;
        if (targetDay.IsOtherMonth) return;

        if (!e.DataView.Properties.TryGetValue("DraggedItem", out object draggedItem)) return;

        if (draggedItem is TaskCompletionTime session)
        {
            if (viewModel.CanInsertSessionAt(targetDay, insertIndex, session))
            {
                await viewModel.MoveSessionToDay(targetDay, insertIndex, session);
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }
        else if (draggedItem is Mission mission)
        {
            if (IsDateAllowedByDeadline(targetDay.Date.Date, e.DataView.Properties)
                && viewModel.CanInsertMissionAt(targetDay, insertIndex, mission))
            {
                await viewModel.CreateSessionFromMission(targetDay, insertIndex, mission);
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        ResetAllMargins(listView);
        ResetAllDaysBackground();
        viewModel.OnDragOver(-1);
        ClearLocalHint();
    }
    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);

        while (parent != null)
        {
            if (parent is T typedParent)
                return typedParent;

            parent = VisualTreeHelper.GetParent(parent);
        }

        return default;
    }

    private void TaskItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Grid grid)
            grid.Opacity = 0.6;
    }

    private void TaskItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Grid grid)
            grid.Opacity = 1.0;
    }

    private void InboxMission_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        var stackPanel = sender as StackPanel;
        var mission = stackPanel?.DataContext as Mission;

        if (mission != null)
        {
            args.Data.Properties.Add("DraggedItem", mission);
            bool isInboxMission = !mission.StartDate.HasValue || !mission.EndDate.HasValue;
            args.Data.Properties.Add("SourceIsInbox", isInboxMission);
            args.Data.RequestedOperation = DataPackageOperation.Move;

            var projectAncestor = FindProjectAncestor(mission);
            if (projectAncestor?.StartDate != null)
            {
                if (projectAncestor.StartDate.Value.Date == projectAncestor.EndDate?.Date)
                {
                    args.Data.Properties.Add("DragMaxDate", projectAncestor.EndDate.Value.Date);
                    args.Data.Properties.Add("DragIsDeadlineOnly", true);
                }
                else
                {
                    args.Data.Properties.Add("DragMinDate", projectAncestor.StartDate.Value.Date);
                    args.Data.Properties.Add("DragMaxDate", projectAncestor.EndDate.Value.Date);
                    args.Data.Properties.Add("DragIsDeadlineOnly", false);
                }
            }
        }
    }

    private void OpenEditTaskFrame(object sender, DoubleTappedRoutedEventArgs e)
    {
        var grid = sender as Grid;
        if (grid == null) return;
        var session = grid.DataContext as TaskCompletionTime;
        if (session?.IdMissionNavigation != null)
        {
            ViewModelStore.GetInstance().CalendarViewModel?.OpenTaskEditor(session.IdMissionNavigation);
        }
    }

    private void SetLocalHint(string? message = null)
    {
        DragHint.Text = message ?? "перенесите задачу на календарь";
    }

    private void ClearLocalHint()
    {
        DragHint.Text = string.Empty;
    }

    private static Mission? FindProjectAncestor(Mission mission)
    {
        var current = mission;
        while (current != null)
        {
            if (current.IsProject)
                return current;
            current = current.IdUpMissionNavigation;
        }
        return null;
    }

    /*private void OpenNewTaskFrame_Click(object sender, RoutedEventArgs e)
    {
        //if (!_isNewTaskFrameInitialized)
        //{
        NewTaskFrame.Navigate(typeof(SelectedAndNewTask));
        //    _isNewTaskFrameInitialized = true;
        //}

        NewTaskFrameHost.Visibility = Visibility.Visible;
        InboxTree.Visibility = Visibility.Collapsed;
    }

    private void CloseNewTaskFrame_Click(object sender, RoutedEventArgs e)
    {
        NewTaskFrameHost.Visibility = Visibility.Collapsed;
        InboxTree.Visibility = Visibility.Visible;
        var viewModel = this.DataContext as MonthCalendarViewModel;
        viewModel.RebuildMissionBuckets();
        viewModel.RefreshAllDays();
    }*/

    private static bool IsDateAllowedByDeadline(DateTime targetDate, Windows.ApplicationModel.DataTransfer.DataPackagePropertySetView properties)
    {
        DateTime? minDate = null, maxDate = null;
        bool isDeadlineOnly = false;

        if (properties.TryGetValue("DragMaxDate", out var maxObj) && maxObj is DateTime max)
            maxDate = max;
        if (maxDate == null) return true;

        if (properties.TryGetValue("DragMinDate", out var minObj) && minObj is DateTime min)
            minDate = min;
        if (properties.TryGetValue("DragIsDeadlineOnly", out var dlObj) && dlObj is bool dl)
            isDeadlineOnly = dl;

        if (isDeadlineOnly)
            return targetDate <= maxDate.Value;

        return minDate != null && targetDate >= minDate.Value && targetDate <= maxDate.Value;
    }
}
