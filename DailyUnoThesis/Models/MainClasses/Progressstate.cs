using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class Progressstate
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Category>? Categories { get; set; } = new List<Category>();

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();
}
