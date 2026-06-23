using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DailyUnoThesis.Models.MainClasses
{
    public partial class Invitation : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int IdFromUser { get; set; }

        public int IdToUser { get; set; }

        public int IdProject { get; set; }

        public virtual User IdFromUserNavigation { get; set; } = null!;

        public virtual Category IdProjectNavigation { get; set; } = null!;

        public virtual User IdToUserNavigation { get; set; } = null!;

        private bool _isDelete;
        public bool IsDelete
        {
            get => _isDelete;
            set { _isDelete = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
