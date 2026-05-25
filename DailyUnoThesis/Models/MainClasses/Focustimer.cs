using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class Focustimer
{
    public int Id { get; set; }

    public string? TitleMission { get; set; }

    public int? Repetitions { get; set; }

    public int? CountRound { get; set; }

    public TimeOnly DurationTime { get; set; }

    public TimeOnly? DurationBreak { get; set; }

    public TimeOnly? LongBreakTime { get; set; }

    public int UserId { get; set; }

    public int? MissionId { get; set; }

    public bool? IsRound { get; set; }

    public virtual Mission? Mission { get; set; }

    public virtual User? User { get; set; } = null!;
}
