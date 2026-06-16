using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls
{
    public partial class TableCalendarViewModel : ObservableObject
    {
        public DispatcherQueue? Dispatcher { get; set; }

        [ObservableProperty]
        private Frame _calendarViewFrame;
        [ObservableProperty]
        private ObservableCollection<Mission> _inboxTreeMissions = new();

        [ObservableProperty]
        private Mission _selectedMission;

        [ObservableProperty]
        private Visibility _newTaskFrameVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _inboxVisibility = Visibility.Visible;

        private readonly TaskStateService _taskState = TaskStateService.GetInstance();
        private Frame _newTaskFrame;
        private Border _newTaskFrameHost;
        private UIElement _inboxTree;
        public ObservableCollection<Mission> PlannedMissions { get; set; } = new ObservableCollection<Mission>();
        public ObservableCollection<Mission> InboxMissions { get; set; } = new ObservableCollection<Mission>();
        public TableCalendarViewModel()
        {
            _taskState.TasksChanged += OnTasksChanged;
        }
        public async Task LoadDataFromApi()
        {
            try
            {
                await _taskState.LoadAsync();
                if (Dispatcher != null)
                    Dispatcher.TryEnqueue(async () => await BuildInboxTreeMissions());
                else
                    await BuildInboxTreeMissions();
            }
            catch (Exception ex)
            {
                ;
            }
        }

        private void OnTasksChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (Dispatcher != null)
                Dispatcher.TryEnqueue(async () => await BuildInboxTreeMissions());
            else
                _ = BuildInboxTreeMissions();
        }

        public void RegisterNewTaskFrame(Frame frame)
        {
            _newTaskFrame = frame;
        }
        public void RegisterNewTaskFrameHost(Border host, UIElement inboxTree)
        {
            _newTaskFrameHost = host;
            _inboxTree = inboxTree;
        }

        public ICommand OpenNewTaskCommand => new RelayCommand(() =>
        {
            _newTaskFrame?.Navigate(typeof(SelectedAndNewTask));
            NewTaskFrameVisibility = Visibility.Visible;
            InboxVisibility = Visibility.Collapsed;
        });

        public ICommand CloseNewTaskCommand => new RelayCommand(async () =>
        {
            NewTaskFrameVisibility = Visibility.Collapsed;
            InboxVisibility = Visibility.Visible;
            await BuildInboxTreeMissions(); 
        });

        public void OpenTaskEditor(Mission mission)
        {
            ViewModelStore.GetInstance().DetailedTask = null;
            _newTaskFrame?.Navigate(typeof(SelectedAndNewTask));
            ViewModelStore.GetInstance().DetailedTask?.GetTask(mission);

            if (_newTaskFrameHost != null)
                _newTaskFrameHost.Visibility = Visibility.Visible;
            if (_inboxTree != null)
                _inboxTree.Visibility = Visibility.Collapsed;
        }

        public async Task BuildInboxTreeMissions()
        {
            var result = new ObservableCollection<Mission>();
            foreach (var root in _taskState.Tasks.Where(IsRootMission))
            {
                if (SubtreeContainsInboxMission(root, new HashSet<int>()))
                    result.Add(root);
            }
            
            InboxTreeMissions = result;
        }

        private bool IsRootMission(Mission mission)
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
        private static bool IsPlannedMission(Mission mission)
        {
            if (mission.StartDate != null && mission.EndDate != null && mission.StartDate != DateTime.MinValue && mission.EndDate != DateTime.MinValue)
            {
                if (mission.StartDate.Value.Date == mission.EndDate.Value.Date)
                    return true;
                else if (mission.TaskCompletionTimes is not null && mission.TaskCompletionTimes.Count() > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private static bool IsInboxMission(Mission mission)
        {
            return !IsPlannedMission(mission);
        }

       
    }
}
