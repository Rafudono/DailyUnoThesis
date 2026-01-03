
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using Windows.UI.Core;
namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskPages.xaml
    /// </summary>
    public partial class TaskPages : Page
    {
        PageNavigation Navigation;
        TaskPages pass;
        string test {  get; set; }  
        public TaskPages()
        {
            this.InitializeComponent();
            //Navigation = pageNavigation;
            pass = this;
            DataContext = TaskPageControle.GetInstance();   
            var en = DataContext as TaskPageControle;
            en.SetDispatcher(Dispatcher);
            en?.SetControl(pass);
            en.GetLists();
            Task.Run(async () => { await GetTaskCatPage(en); });
        }

        private async Task GetTaskCatPage(TaskPageControle? en)
        {
            try
            {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await en.GetListPage();
                });
                //или так, но без асинхронности
                //DispatcherQueue.TryEnqueue(() =>
                //{
                //    en.GetListPage();
                //});
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }



        }
        public void CloseCatBannerClass()
        {
             //CatBanner.Visibility = Visibility.Collapsed;         
        }


        private void CloseCatBanner(object sender, RoutedEventArgs e)
        {
            //  CatBanner.Visibility = Visibility.Collapsed;
            test = "заполнен";
        }

        private void OpenCatBanner(object sender, RoutedEventArgs e)
        {
            //CatBanner.Visibility = Visibility.Visible;
            test = "заполнен";
        }

        private void CloseCatBanner(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
