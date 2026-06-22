using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DailyUnoThesis.Presentation.ViewModel.DashboardControl;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.ProjectControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DailyUnoThesis.Presentation.View.Dashboard;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel;

    public DashboardPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
    }

    private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {

        // Проверяем, что DataContext — это наша ViewModel
        if (args.NewValue is DashboardViewModel viewModel)
        {
            viewModel.SetDispatcher(this.Dispatcher);

            ViewModel = viewModel;
            viewModel.SetControl(this);

            ViewModelStore.GetInstance().PanelDashboard = viewModel;
        }
    }
}
