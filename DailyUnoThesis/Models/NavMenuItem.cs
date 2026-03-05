using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models;
public record NavMenuItem(
    string Label,
    string? Icon = null,
    string? Route = null,
    object? Data = null,
    bool IsHeader = false,
    bool IsSeparator = false
);
