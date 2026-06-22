using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyUnoThesis.Models.MainClasses;

public partial class TaskCompletionTime : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int Id { get; set; }

    public int IdMission { get; set; }

    private DateTime? _startExecution;
    public DateTime? StartExecution
    {
        get => _startExecution;
        set
        {
            if (_startExecution != value)
            {
                _startExecution = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedTime));
            }
        }
    }

    private DateTime? _endExecution;
    public DateTime? EndExecution
    {
        get => _endExecution;
        set
        {
            if (_endExecution != value)
            {
                _endExecution = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedTime));
            }
        }
    }

    public virtual Mission? IdMissionNavigation { get; set; } = null!;

    public string FormattedTime
    {
        get
        {
            if (StartExecution.HasValue && EndExecution.HasValue)
            {
                if (StartExecution.Value.TimeOfDay == new TimeSpan(0, 0, 1))
                    return string.Empty;
                if (StartExecution.Value == EndExecution.Value)
                    return StartExecution.Value.ToString("HH:mm");
                return $"{StartExecution.Value:HH:mm}-{EndExecution.Value:HH:mm}";
            }
            else if (StartExecution.HasValue)
            {
                return StartExecution.Value.ToString("HH:mm");
            }
            return string.Empty;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
