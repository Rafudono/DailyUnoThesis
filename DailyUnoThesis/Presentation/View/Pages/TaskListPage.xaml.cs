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
        public TaskListControle ViewModel { get; } = new();
        TaskListPage pass;
        public TaskListPage()
        {
            this.InitializeComponent();
            pass = this;
            DataContext = ViewModel;
            ViewModel.SetDispatcher(Dispatcher);
            ViewModel?.SetControl(this);

        }

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

        //public async Task GetTodayPage()
        //{
        //    try
        //    {
        //      await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //        {
        //            var en = DataContext as TaskListControle;
        //             await en.GetToday();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //      var dialog = new ContentDialog{Title = "Ошибка", Content = ex, CloseButtonText = "Закрыть" };  await dialog.ShowAsync();
        //    }
        //}

        //public async Task GetCompletePage()
        //{
        //    try
        //    {
        //      await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //        {
        //            var en = DataContext as TaskListControle;
        //            await en.GetComplete();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //      var dialog = new ContentDialog{Title = "Ошибка", Content = ex, CloseButtonText = "Закрыть" };  await dialog.ShowAsync();
        //    }
        //}


    }
}
