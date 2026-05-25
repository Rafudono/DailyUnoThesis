using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyUnoThesis.Presentation.View.Timer
{
    /// <summary>
    /// Логика взаимодействия для PanelTimer.xaml
    /// </summary>
    public partial class PanelTimer : Page
    {
        public PanelTimerControle ViewModel;

        public PanelTimer()
        {
            InitializeComponent();
            var en = DataContext as PanelTimerControle;
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);
            //en.GetTimers();
            //Task.Run(async () => { await GetTaskCatPage(); });
            this.DataContextChanged += OnDataContextChanged;
        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            // Проверяем, что DataContext — это наша ViewModel
            if (args.NewValue is PanelTimerControle viewModel)
            {
                // Передаем DispatcherQueue (в WinUI/Uno 5 это DispatcherQueue)
                //viewModel.SetDispatcher(this.Dispatcher);

                //// Передаем саму View
                //viewModel.SetControl(this);
                //await viewModel.GetCaterogy();
                ViewModel = viewModel;

                //BindingProxy.GetInstance().TaskPageControle = viewModel;
                //ViewModelStore.GetInstance().PanelTask = viewModel;
                //AdaptiveStates.CurrentStateChanged += (s, e) =>
                //{
                //    // Передаем во ViewModel название текущего состояния
                //    ViewModel.CurrentViewState = e.NewState.Name;
                //};
                //BaseList.SelectedItem = BaseList.IndexOf(1);
                //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //{
                // await viewModel.BuildMenu();
                //BaseListView.SelectedItem = viewModel.ListNavigations[0];

                //});
            }
        }



        private async Task GetTaskCatPage()
        {
            try
            {
              await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var en = DataContext as PanelTimerControle;
                    ////en.SetDispatcher(Dispatcher);
                    en.GetPomodoroPage();


                });

            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex.Message);
            }



        }


    }
}
