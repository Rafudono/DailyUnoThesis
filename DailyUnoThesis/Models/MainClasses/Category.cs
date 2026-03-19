using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyUnoThesis.Models.MainClasses;
public partial class Category: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int IdBigBoss { get; set; }
    public int? IdUpCategory { get; set; }
    public virtual Category? IdUpCategoryNavigation { get; set; }
    public virtual ICollection<Category> InverseIdUpCategoryNavigation { get; set; } = new List<Category>();

    public virtual ICollection<Mission>? Missions { get; set; } = new List<Mission>();

    public virtual ICollection<User>? Users { get; set; } = new List<User>();

    private bool? isCheack = false;
    public bool? IsCheack
    {
        get => isCheack;
        set
        {
            if (isCheack != value)
            {
                isCheack = value;
                OnPropertyChanged();
            }
        }
    }
}
