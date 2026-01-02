using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Timer = System.Timers.Timer;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class СountdownTimer : TaskTimer
    {
        private bool breakTimer = false;
        public bool IsEnd { get; set; } = false;
        public bool BreakTimer { get => breakTimer; set { breakTimer = value; Signal(); } }         //public DispatcherTimer BreakTimer { get; set; }

        public int Repetitions { get; set; }
        public int CurrentRepetitions { get; set; }
        public TimeSpan BreakTime { get; set; }
        public override void PauseTimer()
        {
            if (!IsPaused && Timer.IsEnabled)
            {
                
                Timer.Stop(); // Паузируем таймер
                IsPaused = true;
            }
            else
            {
                Timer.Start(); // Продолжаем таймер
                IsPaused = false;
            }
        }

        public void SettingRepeat(TimeSpan breakTime, int repeat)
        {
            BreakTime = breakTime;
            Repetitions = repeat;
            CurrentRepetitions = repeat;
        }

        public override void TimerEvent(object sender, object e)
        {
            if (RemainingTime.TotalMilliseconds > 0)
            {
                RemainingTime -= TimeSpan.FromSeconds(1); // Уменьшение времени на 1 секунду
                 // Оповещаем об изменениях
            }
            else
            {
                Timer.Stop(); // Останавливаем таймер
                //MessageBox.Show("Таймер завершил работу!");
                if (BreakTimer == false)
                {
                    if (CurrentRepetitions > 0)
                    {
                        CurrentRepetitions--;
                        BreakTimer = true;
                        RemainingTime = BreakTime;
                        Timer.Start();
                    }
                }
                else
                {
                    BreakTimer = false;
                    RemainingTime = SpecifiedTime;
                    Timer.Start();
                }

            }
            //return RemainingTime;        // Оповещаем об изменениях
        }


        //public virtual void SettingBreakTimer()
        //{
        //    BreakTimer = new DispatcherTimer(); // Таймер срабатывает каждую секунду
        //    BreakTimer.Tick += new EventHandler(TimerBreakEvent);
        //    BreakTimer.Interval = TimeSpan.FromSeconds(1);
        //    RemainingBreakTime = BreakTime;
            
        //    //BreakTimer.Start();
        //    IsPaused = false;
        //}

        //public void TimerBreakEvent(object sender, EventArgs e)
        //{
        //    if (RemainingTime.TotalMilliseconds > 0)
        //    {
        //        RemainingTime -= TimeSpan.FromSeconds(1); // Уменьшение времени на 1 секунду
        //                                                  // Оповещаем об изменениях
        //    }
        //    else
        //    {
        //        Timer.Stop(); // Останавливаем таймер
        //        MessageBox.Show("Таймер завершил работу!");

        //    }
        //    //return RemainingTime;        // Оповещаем об изменениях
        //}


    }
}
