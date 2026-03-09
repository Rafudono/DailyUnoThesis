using System;
using System.Collections.Generic;

namespace DailyUnoThesis.Models.MainClasses;
public partial class Message
{
    public int Id { get; set; }

    public int? IdFrom { get; set; }

    public int IdAddressedTo { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? IdCategory { get; set; }
}
