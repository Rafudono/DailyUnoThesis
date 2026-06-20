using System;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Models.AuthModels;
public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public User User { get; set; }
}
