using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DailyUnoThesis.Models.MainClasses;
public partial class CalendarDay : ObservableObject
{

    public DateTime Date { get; set; }
    public int DayNumber => Date.Day;
    public bool IsToday { get; set; }
    public bool IsOtherMonth { get; set; }
    public bool IsDragOver { get; set; }

    [ObservableProperty]
    private ObservableCollection<Mission> _tasks = new ObservableCollection<Mission>();

    [ObservableProperty]
    private ObservableCollection<TaskCompletionTime> _tasksSessions = new ObservableCollection<TaskCompletionTime>();
}

