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
        public OverdueTaskControle ViewModel { get; } = new OverdueTaskControle();
        public TaskOverdueListPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            ViewModel.SetDispatcher(Dispatcher);
            ViewModel?.SetControl(this);
        }
    }
}
