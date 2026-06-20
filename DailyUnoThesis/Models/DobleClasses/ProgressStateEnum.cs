using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public enum ProgressStateEnum
{
    Waiting = 1,
    Assigned = 2,
    InProgress = 3,
    Completed = 4,
    Paused = 5,
    Failed = 6,
    Abandoned = 7
}
