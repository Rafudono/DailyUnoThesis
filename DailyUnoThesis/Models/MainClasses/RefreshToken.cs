using System;
using System.Collections.Generic;
using DailyUnoThesis.Models.MainClasses;

namespace DailyThesisAPI;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Revoked { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
