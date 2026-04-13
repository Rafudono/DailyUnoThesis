using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
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

namespace DailyUnoThesis.Presentation.View.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectedAndNewTask : Page
    {
        public TaskViewModel ViewModel;
        public SelectedAndNewTask()
        {
            this.InitializeComponent();
            //DataContext = new TaskViewModel();
            //var vm = DataContext as TaskViewModel;
            this.DataContextChanged += OnDataContextChanged;
            //PageNavigation.GetInstance().GetPageCategory(this);
            Presentation.ViewModel.HelperClasses.ViewModelStore.GetInstance().GetPageTask(this);
            ViewModel = Presentation.ViewModel.HelperClasses.ViewModelStore.GetInstance().DetailedTask;
        }
        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is TaskViewModel viewModel)
            {
                viewModel.SetDispatcher(this.Dispatcher);
                viewModel.SetControl(this);
                ViewModel = viewModel;

            }
        }

        private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            // Обновляем дату во ViewModel при клике на календарь
            ViewModel.SelectedDate = args.AddedDates.FirstOrDefault();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
