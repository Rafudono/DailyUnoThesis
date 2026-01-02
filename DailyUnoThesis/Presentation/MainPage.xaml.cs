using System;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;

namespace DailyUnoThesis.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = PageNavigation.GetInstance();
        var en = DataContext as PageNavigation;
        Task.Run(async () => { await GetClassPage(en); });
    }

    private async Task GetClassPage(PageNavigation? en)
    {
        try
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                en.GetClass();
            });
        }
        catch (Exception ex)
        {

        }
    }
}
