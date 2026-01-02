using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;

public partial class Mission
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string Title { get; set; } = null!;

    public bool? IsComplete { get; set; } = false;   

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? IdNotification { get; set; }

    public int? CategoryId { get; set; }

    public int UserId { get; set; }

    public int? NotificationId { get; set; }

    public int? IdUpMission { get; set; }

    public int? TagId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Mission? IdUpMissionNavigation { get; set; }

    public virtual ICollection<Mission> InverseIdUpMissionNavigation { get; set; } = new List<Mission>();

    public virtual Notification? Notification { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Tag? Tag { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
    public virtual int? LevelUp { get; set; } = 0;  

    public static implicit operator List<object>(Mission v)
    {
        throw new NotImplementedException();
    }
}
