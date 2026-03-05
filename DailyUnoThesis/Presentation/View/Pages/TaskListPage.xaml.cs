using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Popups;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskListPage.xaml
    /// </summary>
    public partial class TaskListPage : Page
    {
        public TaskListControle ViewModel;
        TaskListPage pass;
        public TaskListPage()
        {
            this.InitializeComponent();
            pass = this;
            //DataContext = ViewModel;
            var en = DataContext as TaskListControle;
            ViewModel = en;
            en?.SetDispatcher(Dispatcher);
            en?.SetControl(this);
            //this.DataContextChanged += OnDataContextChanged;

        }



        //private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        //{


            
        //    if (args.NewValue is TaskListControle viewModel)
        //    {
        //        // Передаем DispatcherQueue (в WinUI/Uno 5 это DispatcherQueue)
        //        viewModel.SetDispatcher(this.Dispatcher);

        //        // Передаем саму View
        //        viewModel.SetControl(this);
        //        viewModel.GetCaterogy();
        //        //BaseList.SelectedItem = BaseList.IndexOf(1);
        //        //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //        //{
        //        // await viewModel.BuildMenu();
        //        //BaseListView.SelectedItem = viewModel.ListNavigations[0];
        //        ;
        //        //});
        //    }
        //}

        public async Task GetTaskCatPage()
        {
            try
            {
              await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var en = DataContext as TaskListControle;
                    await en.FillData();
                });
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog{Title = "Ошибка", Content = ex, CloseButtonText = "Закрыть" };  
                await dialog.ShowAsync();
            }
        }

        //private void ToggleButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (pass != null)
        //    { }
        //}

        //public async Task GetTodayPage()
        //{
        //    try
        //    {
        //        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
        //          {
        //              var en = DataContext as TaskListControle;
        //              await en.GetToday();
        //          });
        //    }
        //    catch (Exception ex)
        //    {
        //        var dialog = new ContentDialog { Title = "Ошибка", Content = ex, CloseButtonText = "Закрыть" }; await dialog.ShowAsync();
        //    }
        //}

        //public async Task GetCompletePage()
        //{
        //    try
        //    {
        //        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
        //          {
        //              var en = DataContext as TaskListControle;
        //              await en.GetComplete();
        //          });
        //    }
        //    catch (Exception ex)
        //    {
        //        var dialog = new ContentDialog { Title = "Ошибка", Content = ex, CloseButtonText = "Закрыть" }; await dialog.ShowAsync();
        //    }
        //}


    }
}
