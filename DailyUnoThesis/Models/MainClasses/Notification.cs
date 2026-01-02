using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class Notification
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public DateTime Start { get; set; }

    public int MissionId { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();
}
