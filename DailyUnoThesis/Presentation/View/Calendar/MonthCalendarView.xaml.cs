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

        // Пытаемся получить как сессию
        var session = stackPanel?.DataContext as TaskCompletionTime;
        if (session != null)
        {
            // Запрещаем перетаскивание сессий из прошедших дней
            if (session.StartExecution.HasValue && session.StartExecution.Value.Date < DateTime.Today.Date)
            {
                e.Cancel = true;
                return;
            }

            e.Data.Properties.Add("DraggedItem", session);
            e.Data.RequestedOperation = DataPackageOperation.Move;
            return;
        }

        // Пытаемся получить как миссию (из Inbox)
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

        var listView = sender as ListView;
        if (listView == null) return;

        var position = e.GetPosition(listView);

        var grid = FindParent<Grid>(listView);
        if (grid?.DataContext is not CalendarDay targetDay) return;

        // Сначала сбрасываем фон у всех дней
        ResetAllDaysBackground();

        // Проверяем, можно ли вообще вставлять в этот день
        bool isDayAvailable = targetDay.Date.Date >= DateTime.Today.Date;

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

        if (!e.DataView.Properties.TryGetValue("DraggedItem", out object draggedItem)) return;
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

                    // 1. Восстанавливаем фон
                    container.SetValue(Grid.BackgroundProperty,
                        converter.Convert(day.IsOtherMonth, typeof(Brush), bgColor, null));

                    // 2. Восстанавливаем Margin (как в XAML: Margin="2")
                    container.SetValue(FrameworkElement.MarginProperty, new Thickness(2));

                    // 3. Восстанавливаем CornerRadius (как в XAML: CornerRadius="4")
                    if (container is Grid grid)
                    {
                        grid.CornerRadius = new CornerRadius(4);
                    }
                    // 4. Убираем возможный красный фон, если был установлен для недоступного дня
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
        ResetAllDaysBackground();
        var viewModel = this.DataContext as MonthCalendarViewModel;
        viewModel?.OnDragOver(-1);
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

        var listView = sender as ListView;
        if (listView == null) return;

        int insertIndex = viewModel.DropTargetIndex;
        if (insertIndex < 0) return;

        var grid = FindParent<Grid>(listView);
        if (grid?.DataContext is not CalendarDay targetDay) return;

        if (!e.DataView.Properties.TryGetValue("DraggedItem", out object draggedItem)) return;

        // Обработка в зависимости от типа
        if (draggedItem is TaskCompletionTime session)
        {
            // Перемещение существующей сессии внутри календаря
            if (viewModel.CanInsertSessionAt(targetDay, insertIndex, session))
            {
                await viewModel.MoveSessionToDay(targetDay, insertIndex, session);
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }
        else if (draggedItem is Mission mission)
        {
            // Создание новой сессии из миссии Inbox
            if (viewModel.CanInsertMissionAt(targetDay, insertIndex, mission))
            {
                await viewModel.CreateSessionFromMission(targetDay, insertIndex, mission);
                e.AcceptedOperation = DataPackageOperation.Move;
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
            args.Data.Properties.Add("DraggedItem", mission);
            bool isInboxMission = !mission.StartDate.HasValue || !mission.EndDate.HasValue;
            args.Data.Properties.Add("SourceIsInbox", isInboxMission);
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }
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
}
