using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
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
        //public TaskTodayControle ViewModel { get; } = new();
        public TaskTodayControle ViewModel;
        TaskTodayListPage pass;
        public TaskTodayListPage()
        {
            this.InitializeComponent();
            var en = DataContext as TaskTodayControle;
            en?.SetDispatcher(Dispatcher);
            en?.SetControl(this);
            pass = this;
            //DataContext = ViewModel;

            this.DataContextChanged += OnDataContextChanged;

        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            if (args.NewValue is TaskTodayControle viewModel)
            {
                ViewModelStore.GetInstance().TodayTasks = viewModel;
                ViewModel = viewModel;
            }
        }

    }
}
