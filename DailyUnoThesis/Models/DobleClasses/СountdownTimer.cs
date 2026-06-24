using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Timer = System.Timers.Timer;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class СountdownTimer : TaskTimer
    {
        private bool breakTimer = false;
        private bool isRound = true;
        private int repetitions;
        private int currentRepetitions;
        private int repetitionsRound;
        private int currentRepetitionsRound;
        private int repetitionsInRound;
        private int currentRepetitionsInRound;

        private bool isEnd { get; set; } = false;
        public bool IsEnd { get => isEnd; set { isEnd = value; Signal(); } }
        public bool BreakTimer { get => breakTimer; set { breakTimer = value; Signal(); } }

        //public DispatcherTimer BreakTimer { get; set; }

        // расчёт в томатах
        public int Repetitions { get => repetitions; set { repetitions = value; Signal(); } }
        public int CurrentRepetitions { get => currentRepetitions; set { currentRepetitions = value; Signal(); } }

        //расчёт в раундах
        public int RepetitionsRound { get => repetitionsRound; set { repetitionsRound = value; Signal(); } }
        public int CurrentRepetitionsRound { get => currentRepetitionsRound; set { currentRepetitionsRound = value; Signal(); } }

        //томаты в раунде(до большого перерыва)
        public int RepetitionsInRound { get => repetitionsInRound; set { repetitionsInRound = value; Signal(); } }
        public int CurrentRepetitionsInRound { get => currentRepetitionsInRound; set { currentRepetitionsInRound = value; Signal(); } }


        public TimeSpan BreakTime { get; set; }
        public TimeSpan LongBreakTime { get; set; }



        private bool withTheSecondTimer { get; set; } = false;
        public bool WithTheSecondTimer { get => withTheSecondTimer; set { withTheSecondTimer = value; Signal(); } }
        private bool absoluteEnd { get; set; } = false;
        public bool AbsoluteEnd { get => absoluteEnd; set { absoluteEnd = value; Signal(); } }
        private bool stopForBreak { get; set; } = true;
        public bool StopForBreak { get => stopForBreak; set { stopForBreak = value; Signal(); } }




        public override void PauseTimer()
        {
            if ( !IsPaused && Timer.IsEnabled)
            {
                
                Timer.Stop(); // Паузируем таймер
                IsPaused = true;
            }
            else
            {
                Timer.Start(); // Продолжаем таймер
                IsPaused = false;
            }
            Restart = IsPaused;
        }

        public void SettingRepeat(TimeSpan breakTime, TimeSpan longBreakTime, int tomatosInRounds, int tomatos, int rounds, bool isRounds)
        {
            BreakTime = breakTime;
            LongBreakTime = longBreakTime;

            if (isRounds)
            {
                RepetitionsRound = rounds;
                //CurrentRepetitionsRound = rounds;
            }
            else
            {
                Repetitions = tomatos;
                //CurrentRepetitions = tomatos;
            }
            //CurrentRepetitionsInRound = tomatosInRounds;
            RepetitionsInRound = tomatosInRounds;

            isRound = isRounds;

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
                    if (isRound)
                    {
                        CurrentRepetitionsInRound++;
                        if (CurrentRepetitionsInRound < RepetitionsInRound)
                        {
                            RemainingTime = BreakTime;
                            Timer.Start();
                        }
                        else
                        {
                            CurrentRepetitionsRound++;
                            if (CurrentRepetitionsRound < RepetitionsRound)
                            {
                                RemainingTime = LongBreakTime;
                                CurrentRepetitionsInRound = 0;
                                Timer.Start();
                            }
                            else
                            {
                                StopTimer();
                                return;
                            }
                        }
                    }
                    else
                    {
                        CurrentRepetitions++;
                        if (CurrentRepetitions < Repetitions)
                        {
                            CurrentRepetitionsInRound++;
                            if (CurrentRepetitionsInRound < RepetitionsInRound)
                            {
                                RemainingTime = BreakTime;
                                Timer.Start();
                            }
                            else
                            {
                                RemainingTime = LongBreakTime;
                                CurrentRepetitionsInRound = 0;
                                Timer.Start();
                            }
                        }
                        else
                        {
                            StopTimer();
                            return;
                        }
                    }
                    BreakTimer = true;
                    if (WithTheSecondTimer && StopForBreak)
                    {
                        ViewModelStore.GetInstance().Main.StopRegular();
                    }

                    //if (CurrentRepetitions > 0)
                    //{
                    //    CurrentRepetitions--;
                    //    BreakTimer = true;
                    //    RemainingTime = BreakTime;
                    //    Timer.Start();
                    //    return;
                    //}
                }
                else
                {
                    BreakTimer = false;
                    RemainingTime = SpecifiedTime;
                    if (WithTheSecondTimer && StopForBreak)
                    {
                        ViewModelStore.GetInstance().Main.StartRegular();
                    }
                    Timer.Start();
                    return; 
                }
               
            }
            //return RemainingTime;        // Оповещаем об изменениях
        }

        private void StopTimer()
        {
            IsPaused = true;
            Restart = IsPaused;
            IsEnd = true;
            if (WithTheSecondTimer && AbsoluteEnd)
            {
                ViewModelStore.GetInstance().Main.AbsoluteEnd();
            }
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
