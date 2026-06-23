using System;
using DailyUnoThesis.Models;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyThesisAPI.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Uno.Extensions.Navigation;
using Uno.Extensions.Navigation.UI;

namespace DailyUnoThesis.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
    }

    private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (args.NewValue is PageNavigation viewModel)
        {
            ViewModelStore.GetInstance().Main = viewModel;
        }
    }

    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
    }

    private void SideTabBar_SelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
    {
        RootSplitView.IsPaneOpen = false;
    }
}
