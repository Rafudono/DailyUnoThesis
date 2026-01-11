
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;

namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskListPageCategory.xaml
    /// </summary>
    public partial class TaskListPageCategory : Page
    {
        public TaskCategotyControle ViewModel { get; } = new();
        public TaskListPageCategory()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            ViewModel.SetDispatcher(Dispatcher);
            ViewModel?.SetControl(this);
            ViewModel?.FillData();
        }

        public void GetIdCategory(int id)
        {
            var en = DataContext as TaskCategotyControle;
            en?.GetIdCategory(id);
        }
    }
}
