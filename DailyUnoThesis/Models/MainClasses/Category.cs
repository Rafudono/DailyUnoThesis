using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyUnoThesis.Models.MainClasses;
public partial class Category: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

   


    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int IdBigBoss { get; set; }
    public int? IdUpCategory { get; set; }
    public virtual Category? IdUpCategoryNavigation { get; set; }
    public virtual ICollection<Category> InverseIdUpCategoryNavigation { get; set; } = new List<Category>();

    public virtual ICollection<Mission>? Missions { get; set; } = new List<Mission>();

    public virtual ICollection<User>? Users { get; set; } = new List<User>();

    public string? Icon { get; set; }

    public bool IsProgect { get; set; }

    public virtual int? Progress { get; set; } = 0;





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


    // 1. Есть ли подкатегории?
    public bool HasSubCategories => InverseIdUpCategoryNavigation.Any();

    // 2. Есть ли задачи?
    public bool HasMissions => Missions.Any();

    // 3. Есть ли ВООБЩЕ хоть что-то?
    public bool HasAnyContent => HasSubCategories || HasMissions;
    public bool HasAllContent => HasSubCategories && HasMissions;

    // 4. Категория абсолютно пуста? (Для обычного удаления)
    public bool IsEmpty => !HasAnyContent;

    // Метод для обновления всех флагов (вызывать при загрузке или изменениях)
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public void RefreshDeleteMenu()
    {
        OnPropertyChanged(nameof(HasSubCategories));
        OnPropertyChanged(nameof(HasMissions));
        OnPropertyChanged(nameof(HasAnyContent));
        OnPropertyChanged(nameof(IsEmpty));
    }
}
