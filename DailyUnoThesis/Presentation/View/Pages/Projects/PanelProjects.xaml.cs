using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
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

namespace DailyUnoThesis.Presentation.View.Pages.Projects;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PanelProjects : Page
{
    public PanelProjectViewModel ViewModel;

    public Frame framePage;
    public Frame framePageTask;
    public ListView BaseList;
    public ListView CategoriesList;
    PageNavigation Navigation;
    TaskPages pass;
    public Grid GridStatic;
    public TabBar TabBar;
    double acpanel;

    public PanelProjects()
    {
        this.InitializeComponent();
        framePage = ContentFrame;
        GridStatic = TaskContentGrid;
        framePageTask = ContentFrameTask;
        TabBar = new();
        this.DataContextChanged += OnDataContextChanged;

    }

    private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {

        // Проверяем, что DataContext — это наша ViewModel
        if (args.NewValue is PanelProjectViewModel viewModel)
        {
            viewModel.SetDispatcher(this.Dispatcher);

            ViewModel = viewModel;
            viewModel.SetControl(this);
            await viewModel.GetCaterogy();


            ViewModelStore.GetInstance().PanelProject = viewModel;
        }
    }


    private async void Grid_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        Grid g = sender as Grid;
        g.Tag = true;
        //if (sender is FrameworkElement fe)
        //    VisualStateManager.GoToState((Control)fe, "PointerOver", true);
    }

    private void Grid_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        Grid g = sender as Grid;
        g.Tag = false;
    }
}
