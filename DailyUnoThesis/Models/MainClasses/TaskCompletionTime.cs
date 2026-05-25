using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class TaskCompletionTime
{
    public int Id { get; set; }

    public int IdMission { get; set; }

    public DateTime? StartExecution { get; set; }

    public DateTime? EndExecution { get; set; }

    public virtual Mission? IdMissionNavigation { get; set; } = null!;
    public string FormattedTime
    {
        get
        {
            if (StartExecution.HasValue && EndExecution.HasValue)
            {
                return $"{StartExecution.Value:HH:mm}-{EndExecution.Value:HH:mm}";
            }
            else if (StartExecution.HasValue)
            {
                return StartExecution.Value.ToString("HH:mm");
            }
            return string.Empty;
        }
    }
}
