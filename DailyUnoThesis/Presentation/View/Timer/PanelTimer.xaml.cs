using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
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
    /// Логика взаимодействия для PanelTimer.xaml
    /// </summary>
    public partial class PanelTimer : Page
    {
        public PanelTimer()
        {
            InitializeComponent();
            var en = DataContext as PanelTimerControle;
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);
            //en.GetTimers();
            //Task.Run(async () => { await GetTaskCatPage(); });
        }

        private async Task GetTaskCatPage()
        {
            try
            {
              await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var en = DataContext as PanelTimerControle;
                    ////en.SetDispatcher(Dispatcher);
                    en.GetPomodoroPage();


                });

            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex.Message);
            }



        }


    }
}
