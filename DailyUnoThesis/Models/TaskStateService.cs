using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Models;

public enum TaskStateChangeType
{
    Loaded,
    Added,
    Updated,
    Deleted
}

public sealed class TaskStateChangedEventArgs : EventArgs
{
    public TaskStateChangedEventArgs(TaskStateChangeType changeType, Mission? mission = null)
    {
        ChangeType = changeType;
        Mission = mission;
    }

    public TaskStateChangeType ChangeType { get; }
    public Mission? Mission { get; }
}

public class TaskStateService
{
    public event EventHandler<TaskStateChangedEventArgs>? TasksChanged;

    public TaskStateService()
    {
        
    }
    private static TaskStateService instance;
    public static TaskStateService GetInstance()
    {
        if (instance == null)
            instance = new TaskStateService();
        return instance;
    }
    public ObservableCollection<Mission> Tasks { get; set; } = new();
    public async Task LoadAsync()
    {
        var missions = await APIHost.GetInstance().GetMissions();
        Tasks.Clear();
        if (missions != null)
        {
            foreach (var mission in missions)
            {
                SubscribeMission(mission);
                Tasks.Add(mission);
            }
        }

        RaiseTasksChanged(TaskStateChangeType.Loaded);
    }
    public async Task AddAsync(Mission task)
    {
        if (task == null)
            return;
        task.UserId = 1;
        await APIHost.GetInstance().CreateMission(task);
        var savedMission = await APIHost.GetInstance().GetLastMission(task);

        SubscribeMission(task);
        Tasks.Add(task);
        RaiseTasksChanged(TaskStateChangeType.Added, task);
    }
    public async Task UpdateAsync(Mission task)
    {
        if (task == null)
            return;

        await APIHost.GetInstance().EditMission(task);

        var existing = Tasks.FirstOrDefault(m => m.Id == task.Id);
        if (existing == null)
        {
            SubscribeMission(task);
            Tasks.Add(task);
            RaiseTasksChanged(TaskStateChangeType.Updated, task);
            return;
        }

        var index = Tasks.IndexOf(existing);
        if (index >= 0)
        {
            UnsubscribeMission(existing);
            SubscribeMission(task);
            Tasks[index] = task;
            RaiseTasksChanged(TaskStateChangeType.Updated, task);
        }
    }
    public async Task DeleteAsync(int id)
    {
        var existing = Tasks.FirstOrDefault(m => m.Id == id);
        if (existing == null)
            return;

        await APIHost.GetInstance().DeleteMission(existing);
        UnsubscribeMission(existing);
        Tasks.Remove(existing);
        RaiseTasksChanged(TaskStateChangeType.Deleted, existing);
    }

    private void RaiseTasksChanged(TaskStateChangeType changeType, Mission? mission = null)
    {
        TasksChanged?.Invoke(this, new TaskStateChangedEventArgs(changeType, mission));
    }

    private void SubscribeMission(Mission mission)
    {
        mission.PropertyChanged -= OnMissionPropertyChanged;
        mission.PropertyChanged += OnMissionPropertyChanged;
    }

    private void UnsubscribeMission(Mission mission)
    {
        mission.PropertyChanged -= OnMissionPropertyChanged;
    }

    private void OnMissionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is Mission changedMission)
        {
            RaiseTasksChanged(TaskStateChangeType.Updated, changedMission);
        }
    }

}
