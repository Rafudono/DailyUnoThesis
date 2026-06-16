using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls;
public partial class HourSlot   :  ObservableObject
{
    [ObservableProperty]
    private int _hour;

    [ObservableProperty]
    private bool _isExpanded = true; 

    public string Label => $"{Hour:D2}:00";

    public bool IsWorkingHour => Hour >= AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour && Hour <= AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour;
    public bool IsNonWorkingHour => !IsWorkingHour;
    public bool IsWorkingHourStart => Hour == AuthorizedUser.GetInstance().AuthUser.DayStartTime.Value.Hour;    
    public bool IsWorkingHourEnd => Hour == AuthorizedUser.GetInstance().AuthUser.DayEndTime.Value.Hour;
    public double Height => IsWorkingHour || IsExpanded ? 60 : 0;
    partial void OnIsExpandedChanged(bool value)
    {
        OnPropertyChanged(nameof(Height));
    }
}
