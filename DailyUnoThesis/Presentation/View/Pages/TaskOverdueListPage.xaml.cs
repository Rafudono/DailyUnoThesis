using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskOverdueListPage.xaml
    /// </summary>
    public partial class TaskOverdueListPage : Page
    {
        public TaskOverdueListPage()
        {
            InitializeComponent();
            var en=DataContext as OverdueTaskControle;
            en?.SetControl(this);
            en?.SetDispatcher(Dispatcher);
            en?.GetOverdue();

        }
    }
}
