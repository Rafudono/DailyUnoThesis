
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using Uno.Extensions.Specialized;
using Uno.Toolkit.UI;
using Windows.UI.Core;
namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TaskPages.xaml
    /// </summary>
    public partial class TaskPages : Page
    {
        public Frame framePage;
        public Frame framePageTask;
        public ListView BaseList;
        public ListView CategoriesList;
        PageNavigation Navigation;
        TaskPages pass;
        public Grid GridStatic;
        public TaskPageControle ViewModel;
        double acpanel;


        //public TaskPageControle ViewModel { get; } = new();
        string test {  get; set; }  
        public TaskPages()
        {
            this.InitializeComponent();
            pass = this;
            framePage = ContentFrame;
            GridStatic = TaskContentGrid;
            framePageTask = ContentFrameTask;
            //acpanel = FilterSplitView.ActualWidth;
            //BaseList = BaseListView;
            //CategoriesList = CategoriesListView;

            //DataContext = ViewModel;

            //var en = DataContext as TaskPageControle;
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);
            //en?.BuildMenu();
            //en?.GetLists();
            //DataContext = ViewModel;
            //ViewModel.SetDispatcher(Dispatcher);
            //ViewModel?.SetControl(pass);
            //ViewModel.GetLists();
            //Task.Run(async () => { await GetTaskCatPage(ViewModel); });
            this.DataContextChanged += OnDataContextChanged;
            this.SizeChanged += (s, e) =>
            {
                if (DataContext is TaskPageControle vm)
                {
                    // Передаем новую ширину страницы во ViewModel
                    vm.OnWindowSizeChanged(e.NewSize.Width);
                }
            };
        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
          
            // Проверяем, что DataContext — это наша ViewModel
            if (args.NewValue is TaskPageControle viewModel)
            {
                // Передаем DispatcherQueue (в WinUI/Uno 5 это DispatcherQueue)
                viewModel.SetDispatcher(this.Dispatcher);

                // Передаем саму View
                viewModel.SetControl(this);
                await viewModel.GetCaterogy();
                ViewModel = viewModel;

                //BindingProxy.GetInstance().TaskPageControle = viewModel;
                ViewModelStore.GetInstance().PanelTask = viewModel;
                //BaseList.SelectedItem = BaseList.IndexOf(1);
                //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //{
                // await viewModel.BuildMenu();
                //BaseListView.SelectedItem = viewModel.ListNavigations[0];

                //});
            }
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

        private async void SelectorBar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            INavigator _navigator = null;
            var en =DataContext as TaskPageControle;
            if ( en is TaskPageControle viewModel)
            {
                
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    _navigator = en._navigator;
                });
            }


            var selectedItem = sender.SelectedItem;
            // Предположим, в Tag у вас лежит объект Category или ваш NavMenuItem
            if (selectedItem is NavMenuItem navItem )
            {
                if (navItem.Data is Category)
                {
                    await _navigator.NavigateRouteAsync(this, "ContentRegion/CategoryTasks", data: navItem.Data);
                }
                else
                {
                    await _navigator.NavigateRouteAsync(this, $"ContentRegion/{navItem.Route}");
                }
                // Явно вызываем навигатор и передаем данные (selectedCategory)
                // 'this' указывает, что навигация должна быть ВНУТРИ текущего региона (контентной области)
                
            }

        }

        private void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void SelectedAndVisible(TabBar sender, TabBarSelectionChangedEventArgs args)
        {
            if (GridStatic.Visibility == Visibility.Visible)
            {
                return;
            }
            await Task.Delay(100);
            CategoriesRepeater.SelectedItem = null;
            framePage.Visibility = Visibility.Collapsed;
            GridStatic.Visibility = Visibility.Visible;


        }
        public void CollapsedStaticTabBar()
        {

            //StaticTabBar.SelectedIndex = -1;
            //StaticTabBar.SelectedItem = null;
            //StaticTabBar.SelectedIndex = -1;
            //StaticTabBar.UpdateLayout();
            foreach (var item in StaticTabBar.Items.OfType<TabBarItem>())
            {
                item.IsSelected = false;
            }
            StaticTabBar.SelectedIndex = -1;
            StaticTabBar.SelectedItem = null;
        }

        private void novcat(object sender, RoutedEventArgs e)
        {

        }

        //private void OpenFilters_Click(object sender, RoutedEventArgs e)
        //{
        //    //FilterSplitView.Visibility = Visibility.Visible;
        //    FilterSplitView.IsPaneOpen = true;
        //    double sw = FilterSplitView.ActualWidth;
        //    //double cg = TaskContentGrid.ActualWidth;
        //    //TaskContentGrid.Width = cg - sw;
        //}

        //private void CloseFilters_Click(object sender, RoutedEventArgs e)
        //{
        //    FilterSplitView.IsPaneOpen = false;
        //    //FilterSplitView.Visibility = Visibility.Collapsed;
        //    //double sw = FilterSplitView.OpenPaneLength;
        //    //double cg = TaskContentGrid.ActualWidth;
        //    //TaskContentGrid.Width = cg + sw;
        //}

        //private void GoHome_Click(object sender, RoutedEventArgs e)
        //{
        //    ContentFrame.Navigate(typeof(TaskListPage));
        //}

        //private void GoToday_Click(object sender, RoutedEventArgs e)
        //{
        //    ContentFrame.Navigate(typeof(TaskTodayListPage));
        //}
    }
}
