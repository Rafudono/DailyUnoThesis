using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;
using Microsoft.UI.Xaml.Controls;

//using Java.Security.Cert;
using Microsoft.UI.Xaml.Input;
using static System.Net.Mime.MediaTypeNames;

namespace DailyUnoThesis.Presentation.View.Timer
{
    /// <summary>
    /// Логика взаимодействия для Pomodoro.xaml
    /// </summary>
    public partial class Pomodoro : Page

    {
        //public string TextHour {  get; set; }
        //public string TextMin { get; set; }
        //public string TextSec { get; set; }
        public PomodoroTimerControle ViewModel;
        public Pomodoro()
        {
            InitializeComponent();
            //    var en = DataContext as PomodoroTimerControle;
            ////    en.SetDispatcher(Dispatcher);
            //    en?.SetControl(this);
            //    //TextHour = "00";
            //    //TextMin = "00";
            //    //TextSec = "00";
            //    ViewModel = en;
            this.DataContextChanged += OnDataContextChanged;
            this.SizeChanged += MyPage_SizeChanged;

        }

        private void MyPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Page main = (Page)sender;
            double gridActualWidth = GridGrid.ActualWidth; // Получаем актуальную ширину
            double gridActualHeight = main.ActualHeight - 20; // Получаем актуальную высоту
            if (gridActualHeight > gridActualWidth)
            {
                ClockFace.Width =  gridActualWidth - 40;
                ClockFace.Height = gridActualWidth - 40;
                HelperGrid.Height = gridActualWidth - 10;
            }
            else
            { 
                ClockFace.Width =   gridActualHeight - 100;
                ClockFace.Height =  gridActualHeight - 100;
                HelperGrid.Height = gridActualHeight - 100;
            }
            SettingPanel.Width = main.ActualWidth * 0.3;
            SampleTimerList.Height = main.ActualHeight - 30;
            TreeViewMis.Height = main.ActualHeight - 30; 


            double effectiveSize = Math.Min(gridActualWidth, gridActualHeight);

            // Устанавливаем высоту HelperGrid (если он есть, и для чего-то нужен)
            // HelperGrid.Height = effectiveSize - 30; // Ваша старая логика

            // --- Логика изменения FontSize ---
            double baseFontSize = 108; // Начальный максимальный размер шрифта
            double minFontSize = 54;  // Минимальный размер шрифта
            double breakpoint = 380;  // Размер, при котором меняем шрифт
            double maxEffectiveSize = 750;
            double currentFontSize;

            if (effectiveSize > breakpoint)
            {
                // Если размер больше 380, используем максимальный шрифт 
                // (или масштабируем его пропорционально, если нужно плавное изменение)

                // Простой вариант: фиксированный большой шрифт
                currentFontSize = baseFontSize;

                // Плавное масштабирование (опционально):

                //currentFontSize = baseFontSize - (baseFontSize - minFontSize) * (effectiveSize - breakpoint) / (maxEffectiveSize - breakpoint);

                //Вам нужно будет определить 'maxEffectiveSize' — максимальный размер, при котором шрифт 108px.
                // Если вы хотите, чтобы шрифт уменьшался плавно от 108 до 54
                // при уменьшении effectiveSize от, скажем, 600px до 380px:

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

            // Применяем рассчитанный размер шрифта
            TimerDisplay.FontSize = currentFontSize;
            ListSpace.Width = main.ActualWidth * 0.3;

            if (main.ActualWidth - (ClockFace.Width + SettingPanel.Width + 120) >= 350)
            {
                ListSpace.Tag = true;
                
            }
            else
                ListSpace.Tag = false;
            
        }

        private void Time_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Grid main = sender as Grid;
            ////Width = "{Binding ActualWidth, UpdateSourceTrigger=PropertyChanged, ElementName=GridGrid, Converter={StaticResource HeightConventer}}"
            //ClockFace.Width = main.ActualWidth - 30;
            //ClockFace.Height = main.ActualWidth - 30;
            //HelperGrid.Height = main.ActualWidth - 10;// Обновляем свойство, когда размер Grid определен
        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            // Проверяем, что DataContext — это наша ViewModel
            if (args.NewValue is PomodoroTimerControle viewModel)
            {
                // Передаем DispatcherQueue (в WinUI/Uno 5 это DispatcherQueue)
                //viewModel.SetDispatcher(this.Dispatcher);

                // Передаем саму View
                viewModel.SetControl(this);
                //await viewModel.GetCaterogy();
                ViewModel = viewModel;

                //BindingProxy.GetInstance().TaskPageControle = viewModel;
                ViewModelStore.GetInstance().PomodoroTime = viewModel;
              
                //BaseList.SelectedItem = BaseList.IndexOf(1);
                //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //{
                // await viewModel.BuildMenu();
                //BaseListView.SelectedItem = viewModel.ListNavigations[0];

                //});
            }

        }

   
        private void TextBox_OnlyDigits_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            // Разрешаем только цифры
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

     


        private async void TimeTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var current = sender as TextBox;

            // 1. Переход по Enter
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true; // чтобы не было лишних звуков системы
                FocusNext(current);
                SmartFormat(sender as TextBox);
            }

            // 2. Умный Backspace: если бокс пустой, возвращаемся назад
            if (e.Key == Windows.System.VirtualKey.Back && string.IsNullOrEmpty(current.Text))
            {
                FocusPrevious(current);
            }
           
        }

        private void SmartFormat(TextBox tb)
        {
            if (tb == null) return;

            if (tb.Text.Length == 1)
            {
                tb.Text = "0" + tb.Text;
            }
        }

        private void TimeTextBox_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {

            var current = sender as TextBox;
            int maxLength = 2;

            string strVal = current.Text as string;

            // Оставляем только цифры (на случай, если попала буква)
            string cleanString = new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Where(strVal, char.IsDigit)));

            if (int.TryParse(cleanString, out int num))
            {
                // Если передан параметр "Limit59", ограничиваем число
                if (current.Name != "textBox1")
                {
                    
                   if (num >= 60) current.Text = "59";
                   
                }
              
                else if (num > 1) current.Text = "1";

              
            }

            if (current.Text.Length > maxLength)
            {
                string overflow = current.Text.Substring(maxLength);
                current.Text = current.Text.Substring(0, maxLength);

                // Находим следующий TextBox
                var next = GetNextTextBox(current);
                if (next != null)
                {
                    next.Text = overflow + next.Text; // Добавляем в начало следующего
                    if (next.Text.Length > maxLength) next.Text = next.Text.Substring(0, maxLength);

                    next.Focus(FocusState.Programmatic);
                    next.SelectionStart = next.Text.Length;
                }
            }
            // Автопереход, если введено ровно 2 символа
            else if (current.Text.Length == maxLength)
            {
                FocusNext(current);
            }
        }

        // Вспомогательные методы, чтобы не дублировать switch/case
        private void FocusNext(TextBox current)
        {
            var next = GetNextTextBox(current);
            if (next != null)
            {
                next.Focus(FocusState.Programmatic);
                next.SelectAll(); // Выделяем текст, чтобы его было легко заменить
            }
        }

        private void FocusPrevious(TextBox current)
        {
            if (current == textBox2) textBox1.Focus(FocusState.Programmatic);
            else if (current == textBox3) textBox2.Focus(FocusState.Programmatic);
        }

        private TextBox GetNextTextBox(TextBox current)
        {
            if (current == textBox1) return textBox2;
            if (current == textBox2) return textBox3;
            return null;
        }

        private void RemoveCheck(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("yep");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBox1_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {

        }

        private void TimeClassicTextBox_TextChanged(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            //Width = "{Binding Width, ElementName=Test, Converter={StaticResource HeightConventer}}"
            //var wei = Test.Width;
            //var weii = GridGrid.ActualWidth;
            var current = sender as TextBox;
            string strVal = current.Text as string;

            // Оставляем только цифры (на случай, если попала буква)
            string cleanString = new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Where(strVal, char.IsDigit)));

            if (int.TryParse(cleanString, out int num))
            {
                // Если передан параметр "Limit59", ограничиваем число
                if (current.Name == "Classic")
                {
                    if (num > 60) current.Text = "60";
                }
                else if (current.Name == "Short")
                {
                    if (num > 15) current.Text = "15";
                }
                else if (current.Name == "Big")
                {
                    if (num > 60) current.Text = "60";
                }
                else if (current.Name == "TomatoesInRound")
                {
                    if ((bool)Tomatos.IsChecked)
                    {
                        string col = CountTomatos.Text as string;
                        int.TryParse(col, out int cleanCol);
                        //string cleanCol = new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Where(strVal, char.IsDigit)));
                        if (num > cleanCol) current.Text = col;
                    }
                    else
                         if (num > 10) current.Text = "10";
                }
                else
                {
                    if (num > 10) current.Text = "10";
                }

                if (current.Name == "CountTomatos")
                {
                    string colTomat = TomatoesInRound.Text as string;
                    int.TryParse(colTomat, out int cleanColTomat);
                    if (cleanColTomat > num) TomatoesInRound.Text = cleanString;

                }
            }
          
        }

        private void CheckedTomatos(object sender, RoutedEventArgs e)
        {
            string col = CountTomatos.Text as string;
            string colTomat = TomatoesInRound.Text as string;
            int.TryParse(col, out int cleanCol);
            int.TryParse(colTomat, out int cleanColTomat);
            //string cleanCol = new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Where(strVal, char.IsDigit)));
            if (cleanColTomat > cleanCol) TomatoesInRound.Text = col;
        }

        private void Column(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
