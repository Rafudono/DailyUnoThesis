using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class TaskCompletionTime
{
    public int Id { get; set; }

    public int IdMission { get; set; }

    public DateTime? StartExecution { get; set; }

    public DateTime? EndExecution { get; set; }

    public virtual Mission IdMissionNavigation { get; set; } = null!;
}
