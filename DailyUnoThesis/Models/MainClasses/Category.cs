using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;
public partial class Category
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int IdBigBoss { get; set; }
    public int? IdUpCategory { get; set; }
    public virtual Category? IdUpCategoryNavigation { get; set; }
    public virtual ICollection<Category> InverseIdUpCategoryNavigation { get; set; } = new List<Category>();

    public virtual ICollection<Mission>? Missions { get; set; } = new List<Mission>();

    public virtual ICollection<User>? Users { get; set; } = new List<User>();
}
