using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DailyUnoThesis.Models.Conventers;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
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
public sealed partial class TableCalendar : Page
{
    private Grid _highlightedDayGrid;
    private bool _isNewTaskFrameInitialized;
    public TableCalendar()
    {
        this.InitializeComponent();
    }
    private void StackPanel_DragStarting(object sender, DragStartingEventArgs e)
    {
        var stackPanel = sender as StackPanel;
        var mission = stackPanel?.DataContext as Mission;

        if (mission != null && mission.StartDate.HasValue)
        {
            // Запрещаем перетаскивание, если задача в прошедшем дне
            if (mission.StartDate.Value.Date < DateTime.Today.Date)
            {
                e.Cancel = true;
                return;
            }

            e.Data.Properties.Add("DraggedMission", mission);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
        else if (mission != null)
        {
            // Для задач без даты (Inbox) - разрешаем
            e.Data.Properties.Add("DraggedMission", mission);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
    }
    private void PlannedListView_DragOver(object sender, DragEventArgs e)
    {
        var viewModel = this.DataContext as MonthCalendarViewModel;
        if (viewModel == null) return;

        e.AcceptedOperation = DataPackageOperation.Move;

        var listView = sender as ListView;
        if (listView == null) return;

        var position = e.GetPosition(listView);

        if (!e.DataView.Properties.TryGetValue("DraggedMission", out object missionObj)) return;
        var draggedMission = missionObj as Mission;
        if (draggedMission == null) return;

        var grid = FindParent<Grid>(listView);
        if (grid?.DataContext is not CalendarDay targetDay) return;

        // Сначала сбрасываем фон у всех дней
        ResetAllDaysBackground();

        // Проверяем, можно ли вообще вставлять в этот день
        bool isDayAvailable = targetDay.Date.Date >= DateTime.Today.Date && !targetDay.IsOtherMonth;

        if (!isDayAvailable)
        {
            // День недоступен - красная подсветка всего дня
            var dayGrid = FindParent<Grid>(listView);
            if (dayGrid != null)
            {
                _highlightedDayGrid = dayGrid; // Сохраняем ссылку
                dayGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 100, 100));
            }
            viewModel.OnDragOver(-1);
            e.AcceptedOperation = DataPackageOperation.None;
            return;
        }

        // Пустой список
        if (listView.Items.Count == 0)
        {
            ResetAllMargins(listView);
            if (viewModel.CanInsertAt(targetDay, 0, draggedMission))
            {
                viewModel.OnDragOver(0);
            }
            else
            {
                viewModel.OnDragOver(-1);
            }
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

                        if (viewModel.CanInsertAt(targetDay, insertIndex, draggedMission))
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
        if (viewModel.CanInsertAt(targetDay, lastIndex, draggedMission))
        {
            viewModel.OnDragOver(lastIndex);
        }
        else
        {
            viewModel.OnDragOver(-1);
        }
    }
    private void ResetAllDaysBackground()
    {
        if (CalendarDaysGrid == null) return;

        for (int i = 0; i < CalendarDaysGrid.Items.Count; i++)
        {
            var container = CalendarDaysGrid.ContainerFromIndex(i) as FrameworkElement;
            if (container != null)
            {
                // Сбрасываем фон на оригинальный
                var day = CalendarDaysGrid.Items[i] as CalendarDay;
                if (day != null)
                {
                    var converter = new BoolToColorConverter();
                    container.SetValue(Grid.BackgroundProperty,
                        converter.Convert(day.IsOtherMonth, typeof(Brush), "LightGray", null));
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


    private void PlannedListView_DragLeave(object sender, DragEventArgs e)
    {
        var listView = sender as ListView;
        if (listView == null) return;

        ResetAllMargins(listView);

        // Сбрасываем подсветку дня
        if (_highlightedDayGrid != null)
        {
            _highlightedDayGrid.ClearValue(Grid.BackgroundProperty);
            _highlightedDayGrid = null;
        }

        var viewModel = this.DataContext as MonthCalendarViewModel;
        viewModel?.OnDragOver(-1);
    }
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
                    border.Margin = new Thickness(0);
                    border.BorderThickness = new Thickness(0);
                    border.ClearValue(Border.BorderBrushProperty);
                }
            }
        }
    }
    private void PlannedListView_Drop(object sender, DragEventArgs e)
    {
        var viewModel = this.DataContext as MonthCalendarViewModel;
        if (viewModel == null) return;

        var listView = sender as ListView;
        if (listView == null) return;

        int insertIndex = viewModel.DropTargetIndex;
        if (insertIndex < 0) return;

        if (e.DataView.Properties.TryGetValue("DraggedMission", out object missionObj))
        {
            var draggedMission = missionObj as Mission;
            if (draggedMission != null)
            {
                var grid = FindParent<Grid>(listView);
                if (grid?.DataContext is CalendarDay targetDay)
                {
                    // Проверяем, откуда пришла миссия
                    bool isFromInbox = e.DataView.Properties.TryGetValue("SourceIsInbox", out object isInboxObj) && (bool)isInboxObj;

                    if (viewModel.CanInsertAt(targetDay, insertIndex, draggedMission))
                    {

                        viewModel.MoveMissionToDayWithInsert(targetDay, insertIndex, draggedMission, isFromInbox);

                        e.AcceptedOperation = DataPackageOperation.Move;
                    }
                }
            }
        }

        ResetAllMargins(listView);
        ResetAllDaysBackground();
        viewModel.OnDragOver(-1);
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
    private void InboxMission_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        var stackPanel = sender as StackPanel;
        var mission = stackPanel?.DataContext as Mission;

        if (mission != null)
        {
            args.Data.Properties.Add("DraggedMission", mission);
            bool isInboxMission = !mission.StartDate.HasValue || !mission.EndDate.HasValue;
            args.Data.Properties.Add("SourceIsInbox", isInboxMission);
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }
    }

    private void OpenNewTaskFrame_Click(object sender, RoutedEventArgs e)
    {
        if (!_isNewTaskFrameInitialized)
        {
            NewTaskFrame.Navigate(typeof(SelectedAndNewTask));
            _isNewTaskFrameInitialized = true;
        }

        NewTaskFrameHost.Visibility = Visibility.Visible;
        InboxTree.Visibility = Visibility.Collapsed;
    }

    private void CloseNewTaskFrame_Click(object sender, RoutedEventArgs e)
    {
        NewTaskFrameHost.Visibility = Visibility.Collapsed;
        InboxTree.Visibility = Visibility.Visible;
    }
}
