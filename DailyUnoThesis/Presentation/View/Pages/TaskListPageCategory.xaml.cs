
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
        public TaskListPageCategory()
        {
            InitializeComponent();
            //var en = DataContext as TaskCategotyControle;
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);
            //en?.FillData();
        }

        public void GetIdCategory(int id)
        {
            var en = DataContext as TaskCategotyControle;
            en?.GetIdCategory(id);
        }
    }
}
