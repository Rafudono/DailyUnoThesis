using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskOverdueListPage.xaml
    /// </summary>
    public partial class TaskOverdueListPage : Page
    {
        //public OverdueTaskControle ViewModel { get; } = new OverdueTaskControle();
        public TaskOverdueListPage()
        {
            this.InitializeComponent();
            var en = DataContext as OverdueTaskControle;
            en?.SetDispatcher(Dispatcher);
            en?.SetControl(this);
            
            //Task.Run(async () => { await GetTaskCatPage(en); });
        }

        private async Task GetTaskCatPage(OverdueTaskControle? en)
        {
            try
            {
               await  Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    en?.GetOverdue();
                });

            }
            catch (Exception ex)
            {
               
            }



        }

    }
}
