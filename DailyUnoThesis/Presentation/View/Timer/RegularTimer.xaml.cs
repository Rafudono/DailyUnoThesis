
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyUnoThesis.Presentation.View.Timer
{
    /// <summary>
    /// Логика взаимодействия для RegularTimer.xaml
    /// </summary>
    public partial class RegularTimer : Page
    {
        public RegularTimerControle ViewModel { get; } = new RegularTimerControle();
        public RegularTimer()
        {
            InitializeComponent();
            var en = DataContext as RegularTimerControle;
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);


        }
    }
}
