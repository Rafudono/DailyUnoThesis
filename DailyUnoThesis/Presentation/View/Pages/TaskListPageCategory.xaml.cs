
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;

namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskListPageCategory.xaml
    /// </summary>
    public partial class TaskListPageCategory : Page
    {
        //public TaskCategotyControle ViewModel { get; } = new();
        public TaskCategotyControle ViewModel;
        TaskListPageCategory pass;
        private Category Category;
        public TaskListPageCategory()
        {
            this.InitializeComponent();
            
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);
            DataContext = new TaskCategotyControle();
            //var en = DataContext as TaskCategotyControle;
            //ViewModel.SetDispatcher(Dispatcher);
            //ViewModel?.SetControl(this);
            //ViewModel?.FillData();
            this.DataContextChanged += OnDataContextChanged;
            ViewModelStore.GetInstance().GetPageCategory(this);
            //PageNavigation.GetInstance().GetPageCategory(this);

            pass = this;
           

        }

       


        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);

        //    // e.Parameter — это тот самый 'category', который вы передали во фрейм
        //    if (e.Parameter is Category category)
        //    {
        //        Category = category;
        //    }
        //}

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        
            // Проверяем, что DataContext — это наша ViewModel
            if (args.NewValue is TaskCategotyControle viewModel)
            {
                // Передаем DispatcherQueue (в WinUI/Uno 5 это DispatcherQueue)
                viewModel.SetDispatcher(Dispatcher);

                // Передаем саму View
                viewModel.SetControl(this);
                ViewModel = viewModel;
                //PageNavigation.GetInstance().GetPageCategory(this);

            }
        }

        //public void GetIdCategory(int id)
        //{
        //    var en = DataContext as TaskCategotyControle;
        //    en?.GetIdCategory(id);
        //}
    }
}
