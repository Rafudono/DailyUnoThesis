using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyUnoThesis.Models
{
    public class Base : INotifyPropertyChanged
    {
      public void Signal([CallerMemberName] string prop = null) =>
                   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
