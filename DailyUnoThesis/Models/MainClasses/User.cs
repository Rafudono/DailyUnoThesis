using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class User
{
    public int Id { get; set; }

    public string NickName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
