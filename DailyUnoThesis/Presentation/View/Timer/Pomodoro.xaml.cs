using DailyUnoThesis.Presentation.ViewModel.PagesControls;
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
    /// Логика взаимодействия для Pomodoro.xaml
    /// </summary>
    public partial class Pomodoro : Page
    {
        //public string TextHour {  get; set; }
        //public string TextMin { get; set; }
        //public string TextSec { get; set; }
        public PomodoroTimerControle ViewModel { get; } = new PomodoroTimerControle();
        public Pomodoro()
        {
            InitializeComponent();
            var en = DataContext as PomodoroTimerControle;
        //    en.SetDispatcher(Dispatcher);
            en?.SetControl(this);
            //TextHour = "00";
            //TextMin = "00";
            //TextSec = "00";
        }

        //private void MaxLengthChanged(object sender, TextChangedEventArgs e)
        //{
        //    int maxLength = 2;
        //    TextBox textBox = sender as TextBox;
        //    switch (textBox.Name)
        //    {
        //        case "textBox1":
        //            if (textBox1.Text.Length > maxLength)
        //            {
        //                string overflowText = textBox1.Text.Substring(maxLength);
        //                textBox1.Text = textBox1.Text.Substring(0, maxLength);
        //                textBox2.Text += overflowText;
        //                textBox2.Focus();
        //                textBox2.SelectionStart = textBox2.Text.Length;
        //            }
        //            break;
        //        case "textBox2":
        //            if (textBox2.Text.Length > maxLength)
        //            {
        //                string overflowText = textBox2.Text.Substring(maxLength);
        //                textBox2.Text = textBox2.Text.Substring(0, maxLength);
        //                textBox3.Text += overflowText;
        //                textBox3.Focus();
        //                textBox3.SelectionStart = textBox2.Text.Length;
        //            }
        //            break;


        //    }
        //    //if (textBox1.Text.Length > maxLength)
        //    //{
        //    //    string overflowText = textBox1.Text.Substring(maxLength);
        //    //    textBox1.Text = textBox1.Text.Substring(0, maxLength);
        //    //    textBox2.Text += overflowText;
        //    //}
        //}

        //private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // Проверяем, является ли символ цифрой
        //    if (!char.IsDigit(e.Text, 0))
        //    {
        //        // Отменяем ввод, если символ не является цифрой
        //        e.Handled = true;
        //    }
        //}



        //private void NextTextBox(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        TextBox currentTextBox = (TextBox)sender;

        //        if (currentTextBox == textBox1)
        //        {
        //            textBox2.Focus();
        //            textBox2.SelectionStart = textBox2.Text.Length;
        //        }
        //        else if (currentTextBox == textBox2)
        //        {
        //            textBox3.Focus();
        //            textBox3.SelectionStart = textBox2.Text.Length;
        //        }

        //    }
        //}

        //public void VisibilityRepeats(object sender, RoutedEventArgs e)
        //{
        //    if(RepeatPanel.Visibility != Visibility)
        //        RepeatPanel.Visibility = Visibility.Visible;
        //}

        //private void VisibilityRepeats(object sender, RoutedEventArgs e)
        //{
        //    if (RepeatPanel.Visibility == Visibility)
        //        RepeatPanel.Visibility = Visibility.Collapsed;
        //}
        //public void VisibilityRepeatsCol(object sender, RoutedEventArgs e)
        //{
        //    if (RepeatPanel.Visibility == Visibility)
        //        RepeatPanel.Visibility = Visibility.Collapsed;
        //}



        private void RemoveCheck(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("yep");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
