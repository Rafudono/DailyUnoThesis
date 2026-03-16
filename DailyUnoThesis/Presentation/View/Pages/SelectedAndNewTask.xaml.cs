using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DailyUnoThesis.Presentation.View.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SelectedAndNewTask : Page
	{
		public SelectedAndNewTask()
		{
			this.InitializeComponent();
            //DataContext = new TaskViewModel();
            //var vm = DataContext as TaskViewModel;
            this.DataContextChanged += OnDataContextChanged;
            //PageNavigation.GetInstance().GetPageCategory(this);
            ViewModelStore.GetInstance().GetPageTask(this);
        }
        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is TaskViewModel viewModel)
            {
                viewModel.SetDispatcher(this.Dispatcher);
                viewModel.SetControl(this);
                

            }
        }
    }
}
