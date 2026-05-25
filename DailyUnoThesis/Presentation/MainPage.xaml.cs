using System;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;

namespace DailyUnoThesis.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        //DataContext = PageNavigation.GetInstance();
        //var en = DataContext as PageNavigation;
        //Task.Run(async () => { await GetClassPage(en); });
        this.DataContextChanged += OnDataContextChanged;



    }


    private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {

        // Проверяем, что DataContext — это наша ViewModel
        if (args.NewValue is PageNavigation viewModel)
        {
            
            ViewModelStore.GetInstance().Main = viewModel;
           
        }
    }

    private async Task GetClassPage(PageNavigation? en)
    {
        try
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //en.GetClass();
            });
        }
        catch (Exception ex)
        {
            ContentDialog dialog = new ContentDialog
            {
                
                Title = "Внимание",
                Content = ex,
                CloseButtonText = "Ок",
                // В WinUI/Uno обязательно нужно указывать XamlRoot
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult result = await dialog.ShowAsync();
        }
    }


    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
    }


    private void SideTabBar_SelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
    {
        // Как только пользователь выбрал пункт, закрываем панель
        RootSplitView.IsPaneOpen = false;
    }


    //private void TogglePane_Click(object sender, RoutedEventArgs e)
    //{
    //    RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
    //}

    //private void NavListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    // Ваша навигация...

    //    if (NavListView.SelectedItem is NavItem selected)
    //    {
    //        // Переходим на страницу, которую мы прописали в XAML
    //        if (selected.Page != null)
    //            framePage.Navigate(selected.Page);

    //        // Закрываем панель (так как она Overlay)
    //        RootSplitView.IsPaneOpen = false;
    //    }
    //}

}
