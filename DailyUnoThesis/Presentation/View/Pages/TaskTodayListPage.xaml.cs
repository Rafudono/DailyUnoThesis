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
    /// Логика взаимодействия для TaskTodayListPage.xaml
    /// </summary>
    public partial class TaskTodayListPage : Page
    {
        public TaskTodayControle ViewModel { get; } = new();
        public TaskTodayListPage()
        {
            this.InitializeComponent();
            DataContext = this.ViewModel;
            ViewModel.SetDispatcher(Dispatcher);
            ViewModel?.SetControl(this);
        }
    }
}
