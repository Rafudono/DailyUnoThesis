using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class Tag
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string Icon { get; set; } = null!;

    public string? Color { get; set; }

    public virtual ICollection<Mission>? Missions { get; set; } = new List<Mission>();
}
