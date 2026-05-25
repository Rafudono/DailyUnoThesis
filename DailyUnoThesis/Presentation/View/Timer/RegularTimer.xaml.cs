
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;
using Microsoft.UI.Xaml.Controls;

namespace DailyUnoThesis.Presentation.View.Timer
{
    /// <summary>
    /// Логика взаимодействия для RegularTimer.xaml
    /// </summary>
    public partial class RegularTimer : Page
    {
        public RegularTimerControle ViewModel;
        public RegularTimer()
        {
            InitializeComponent();
            //var en = DataContext as RegularTimerControle;
            //en.SetDispatcher(Dispatcher);
            //en?.SetControl(this);
            this.DataContextChanged += OnDataContextChanged;
            this.SizeChanged += MyPage_SizeChanged;


        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            // Проверяем, что DataContext — это наша ViewModel
            if (args.NewValue is RegularTimerControle viewModel)
            {
                // Передаем DispatcherQueue (в WinUI/Uno 5 это DispatcherQueue)
                //viewModel.SetDispatcher(this.Dispatcher);
                ViewModelStore.GetInstance().RegularTimer = viewModel;
                // Передаем саму View
                viewModel.SetControl(this);
                //await viewModel.GetCaterogy();
                ViewModel = viewModel;

                //BindingProxy.GetInstance().TaskPageControle = viewModel;
                //ViewModelStore.GetInstance().RegularTimer = viewModel;

                //BaseList.SelectedItem = BaseList.IndexOf(1);
                //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //{
                // await viewModel.BuildMenu();
                //BaseListView.SelectedItem = viewModel.ListNavigations[0];

                //});
            }

        }

        private void MyPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Page main = (Page)sender;
            double gridActualWidth = GridGrid.ActualWidth; // Получаем актуальную ширину
            double gridActualHeight = main.ActualHeight - 20; // Получаем актуальную высоту

            if (gridActualHeight > gridActualWidth)
            {
                ClockFace.Width = gridActualWidth - 30;
            }
            else
            {
                ClockFace.Width = gridActualHeight - 30;
            }
            ClockFace.Height = gridActualHeight * 0.8;


            double effectiveSize = Math.Min(gridActualWidth, gridActualHeight);

            // --- Логика изменения FontSize ---
            double baseFontSize = 108; // Начальный максимальный размер шрифта
            double minFontSize = 54;  // Минимальный размер шрифта
            double breakpoint = 380;  // Размер, при котором меняем шрифт
            double maxEffectiveSize = 750;
            double currentFontSize;

            if (effectiveSize > breakpoint)
            {
                currentFontSize = baseFontSize;
                double scaleFactor = Math.Max(0, (effectiveSize - breakpoint) / (maxEffectiveSize - breakpoint)); // 600 - пример максимального размера
                currentFontSize = minFontSize + (baseFontSize - minFontSize) * scaleFactor - 5;
                if (currentFontSize >= 115)
                    currentFontSize = 115;
            }
            else
            {
                // Если размер меньше или равен 380, используем минимальный шрифт
                currentFontSize = minFontSize;
            }

            TreeViewMis.Height = main.ActualHeight - 20;

            // Применяем рассчитанный размер шрифта
            TimerDisplay.FontSize = currentFontSize;

            ListSpace.Width = main.ActualWidth * 0.3;

            if (main.ActualWidth - (ClockFace.Width + 80) >= 400)
            {
                ListSpace.Tag = true;

            }
            else
                ListSpace.Tag = false;
        }
    }
}
