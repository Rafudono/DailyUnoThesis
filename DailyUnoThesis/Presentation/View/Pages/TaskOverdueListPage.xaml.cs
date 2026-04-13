using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
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
        public OverdueTaskControle ViewModel;
        TaskOverdueListPage pass;
        public TaskOverdueListPage()
        {
            this.InitializeComponent();
            var en = DataContext as OverdueTaskControle;
            en?.SetDispatcher(Dispatcher);
            en?.SetControl(this);

            //Task.Run(async () => { await GetTaskCatPage(en); });
            pass = this;
         
            this.DataContextChanged += OnDataContextChanged;

        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            if (args.NewValue is OverdueTaskControle viewModel)
            {
                ViewModelStore.GetInstance().OverdueTasks = viewModel;
                ViewModel = viewModel;

                //        ViewModel = viewModel;

                //    //Передаем DispatcherQueue(в WinUI/ Uno 5 это DispatcherQueue)
                //    //viewModel.SetDispatcher(this.Dispatcher);

                //    //Передаем саму View
                //    //viewModel.SetControl(this);
                //    //viewModel.GetCaterogy();
                //    //BaseList.SelectedItem = BaseList.IndexOf(1);
                //    //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //    //{
                //    // await viewModel.BuildMenu();
                //    //BaseListView.SelectedItem = viewModel.ListNavigations[0];
                //    ;
                //    //});
                //}
            }
        }
        //private async Task GetTaskCatPage(OverdueTaskControle? en)
        //{
        //    try
        //    {
        //       await  Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //        {
        //            en?.GetOverdue();
        //        });

        //    }
        //    catch (Exception ex)
        //    {

        //    }



        //}

    }
}
