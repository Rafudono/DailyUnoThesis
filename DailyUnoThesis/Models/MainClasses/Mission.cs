using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyUnoThesis.Models.MainClasses;

public partial class Mission: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int Id { get; set; }

    public string? Description { get; set; }

    public string Title { get; set; } = null!;

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

    public virtual ICollection<Mission> InverseIdUpMissionNavigation { get; set; } = new ObservableCollection<Mission>();

    public virtual Notification? Notification { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Tag? Tag { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
    public virtual int? LevelUp { get; set; } = 0;

    //public static implicit operator List<object>(Mission v)
    //{
    //    throw new NotImplementedException();
    //}

    private bool? _isComplete = false;
    public bool? IsComplete
    {
        get => _isComplete;
        set
        {
            if (_isComplete != value)
            {
                _isComplete = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isRemoving = false;
    public bool IsRemoving
    {
        get => _isRemoving;
        set
        {
            _isRemoving = value;
            OnPropertyChanged();
        }
    }

    private bool _isDelete = false;
    public bool IsDelete
    {
        get => _isDelete;
        set
        {
            _isDelete = value;
            OnPropertyChanged();
        }
    }
    public string FormattedTime
    {
        get
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                return $"{StartDate.Value:HH:mm}-{EndDate.Value:HH:mm}";
            }
            else if (StartDate.HasValue)
            {
                return StartDate.Value.ToString("HH:mm");
            }
            return string.Empty;
        }
    }
}
