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
    private bool _isExpanded = true;  // развернут ли этот час

    public string Label => $"{Hour:D2}:00";

    public bool IsWorkingHour => Hour >= 8 && Hour <= 22;
    public bool IsWorkingHourStart => Hour == 8;    
    public bool IsWorkingHourEnd => Hour == 22;
    // Для нерабочих часов: 2px если свернуто, 60px если развернуто
    public double Height => IsWorkingHour || IsExpanded ? 60 : 2;
    partial void OnIsExpandedChanged(bool value)
    {
        OnPropertyChanged(nameof(Height));
    }
}
