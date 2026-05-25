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
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DailyUnoThesis.Presentation.View.Timer
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public partial class AnalysisTimers : Page
	{
        public AnalysisTimersViewModel ViewModel;
        public AnalysisTimers()
		{
			this.InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            this.SizeChanged += MyPage_SizeChanged;
            this.Loaded += YourPage_Loaded;

        }

        private void MyPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Page main = (Page)sender;
            double gridActualWidth = main.ActualWidth - 20; // Получаем актуальную ширину
            double gridActualHeight = main.ActualHeight - 20; // Получаем актуальную высоту

            if (gridActualHeight > gridActualWidth)
            {
                if (gridActualWidth <= 460)
                {
                    ClockFace.Width = gridActualWidth * 0.3;
                    ClockFace.Height = gridActualWidth * 0.3;
                }
                else
                {
                    ClockFace.Width = gridActualWidth * 0.5;
                    ClockFace.Height = gridActualWidth * 0.5;
                }
            }
            else
            {
                if (gridActualHeight <= 460)
                {
                    ClockFace.Width = gridActualHeight * 0.3;
                    ClockFace.Height = gridActualHeight * 0.3;
                }
                else
                {
                    ClockFace.Width = gridActualHeight * 0.5;
                    ClockFace.Height = gridActualHeight * 0.5;
                }
            }
            //ClockFace.Height = gridActualHeight * 0.3;


            double effectiveSize = Math.Min(gridActualWidth, gridActualHeight);

            // --- Логика изменения FontSize ---
            double baseFontSize = 54; // Начальный максимальный размер шрифта
            double minFontSize = 8;  // Минимальный размер шрифта
            double breakpoint = 450;  // Размер, при котором меняем шрифт
            double maxEffectiveSize = 750;
            double currentFontSize;

            if (effectiveSize > breakpoint)
            {
                currentFontSize = baseFontSize;
                double scaleFactor = Math.Max(0, (effectiveSize - breakpoint) / (maxEffectiveSize - breakpoint)); // 600 - пример максимального размера
                currentFontSize = minFontSize + (baseFontSize - minFontSize) * scaleFactor - 5;
                if (currentFontSize >= 54)
                    currentFontSize = 54;
                if (currentFontSize <= 14)
                    currentFontSize = 14;
            }
            else
            {
                // Если размер меньше или равен 380, используем минимальный шрифт
                currentFontSize = 14;
            }
            PercentageText.FontSize = currentFontSize;

            if(currentFontSize >= 35)
            {
                FullEllipse.StrokeThickness = 35;

            }
            else if(currentFontSize <= 14 && effectiveSize <= 400)
            {
                FullEllipse.StrokeThickness = 8;
            }
            else
            FullEllipse.StrokeThickness = currentFontSize+10;

        }


        private void YourPage_Loaded(object sender, RoutedEventArgs e)
        {

            HeaderScrollViewer?.ChangeView(720, null, null, true);

            // Отписываемся от события Loaded, так как нам нужно это сделать только один раз
            this.Loaded -= YourPage_Loaded;
        }


        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            // Проверяем, что DataContext — это наша ViewModel
            if (args.NewValue is AnalysisTimersViewModel viewModel)
            {
                ViewModel = viewModel;
                viewModel.SetControl(this);

            }

        }
    }
}
