using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using DailyUnoThesis.Models.Conventers;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
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
public sealed partial class TableCalendar : Page
{
    private Grid _highlightedDayGrid;
    private bool _isNewTaskFrameInitialized;
    private TableCalendarViewModel _viewModel;
    public TableCalendar()
    {
        this.InitializeComponent();
        _viewModel = this.DataContext as TableCalendarViewModel;
        if (_viewModel != null)
            _viewModel.Dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        CalendarViewFrame.Navigate(typeof(MonthCalendarView));
    }
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel?.RegisterNewTaskFrame(NewTaskFrame);
        _viewModel?.RegisterNewTaskFrameHost(NewTaskFrameHost, InboxTree);
        ViewModelStore.GetInstance().CalendarViewModel = _viewModel;
        if (_viewModel != null)
            await _viewModel.LoadDataFromApi();
    }
    private void NewTaskFrame_Loaded(object sender, RoutedEventArgs e)
    {
        var frame = sender as Frame;
        //(_viewModel as TableCalendarViewModel)?.RegisterNewTaskFrame(frame);
    }

    private void InboxMission_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        var stackPanel = sender as StackPanel;
        var mission = stackPanel?.DataContext as Mission;

        if (mission == null || mission.IsPlanned)
            return;

        args.Data.Properties.Add("DraggedItem", mission);
        args.Data.RequestedOperation = DataPackageOperation.Move;

        DeadlineHelper.AddDeadlineProperties(args.Data.Properties, mission);
    }
    //private void FrameViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if(WeekViewSelection.IsSelected)
    //        CalendarViewFrame.Navigate(typeof(WeekCalendarView));
    //    else if(MonthViewSelection.IsSelected)
    //        CalendarViewFrame.Navigate(typeof(MonthCalendarView));

    //}

    private void OpenNewTaskFrame_Click(object sender, RoutedEventArgs e)
    {
        NewTaskFrameHost.Visibility = Visibility.Visible;
        InboxTree.Visibility = Visibility.Collapsed;
        NewTaskFrame.Navigate(typeof(SelectedAndNewTask));
    }

    private async void CloseNewTaskFrame_Click(object sender, RoutedEventArgs e)
    {
        NewTaskFrameHost.Visibility = Visibility.Collapsed;
        if (_viewModel != null)
        {
            _viewModel.NewTaskFrameVisibility = Visibility.Collapsed;
            _viewModel.InboxVisibility = Visibility.Visible;
            await _viewModel.BuildInboxTreeMissions();
        }
        InboxTree.ItemsSource = null;
        InboxTree.ItemsSource = _viewModel?.InboxTreeMissions;
        InboxTree.Visibility = Visibility.Visible;
    }
    private void MonthView_Click(object sender, RoutedEventArgs e)
    {
        MonthViewBtn.Style = (Style)Resources["SelectedTabButtonStyle"];
        WeekViewBtn.Style = (Style)Resources["CalendarTabButtonStyle"];
        CalendarViewFrame.Navigate(typeof(MonthCalendarView));
    }

    private void WeekView_Click(object sender, RoutedEventArgs e)
    {
        WeekViewBtn.Style = (Style)Resources["SelectedTabButtonStyle"];
        MonthViewBtn.Style = (Style)Resources["CalendarTabButtonStyle"];
        CalendarViewFrame.Navigate(typeof(WeekCalendarView));
    }
}
