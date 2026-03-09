using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;
public partial class Missionstimer
{
    public int Id { get; set; }

    public string? TitleMission { get; set; }

    public string? DescriptionMission { get; set; }

    public TimeOnly DurationTimer { get; set; }

    public int UserId { get; set; }

    public int? MissionId { get; set; }

    public DateTime Date { get; set; }

    public virtual Mission? Mission { get; set; }

    public virtual User? User { get; set; }
}
