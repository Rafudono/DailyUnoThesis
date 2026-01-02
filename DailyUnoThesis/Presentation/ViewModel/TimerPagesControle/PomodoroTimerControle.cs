using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Presentation.View.Timer;
using Windows.UI.Core;
using Timer = System.Timers.Timer;

namespace DailyUnoThesis.Presentation.ViewModel.TimerPagesControle
{
    public class PomodoroTimerControle:Base
    {

        private СountdownTimer сountdownTimer;
        private string textSec;
        private string textMin;
        private string textHour;
        private string textTime;
        private TimeSpan timeSpan;
        private Pomodoro Pomodoro;
        private CoreDispatcher dispatcher;
        private TimeOnly specifiedTime;
        //public PageNavigation Navigation;
        private Pomodoro TaskPomodoro;
        private List<string> listTime;
        private List<ClassCheckad> listNumderRepeats;
        private string selectedListTime;
        private TimeSpan _remainingTime;
        private ClassCheckad selectedListNumderRepeats;
        private List<ClassCheckad> listTimeBreak;
        private ClassCheckad selectedListTimeBreak;


        public TimeOnly SpecifiedTime
        { get => specifiedTime;
            set
            {
                specifiedTime = value;
                Signal();
            }
        }
        public TimeSpan TimeSpan { get => timeSpan; set { timeSpan = value; Signal(); } }

        public string TextHour { get => textHour; set { textHour = value; Signal(); } }
        public string TextMin { get => textMin; set { textMin = value; Signal(); } }
        public string TextSec { get => textSec; set { textSec = value; Signal();  } }
        public string TextTime { get => textTime; set { textTime = value; Signal(); } }

        public List<ClassCheckad> ListNumderRepeats
        { get => listNumderRepeats;
            set
            {
                listNumderRepeats = value;
                Signal();   
            }
        }
        public ClassCheckad SelectedListNumderRepeats
        {
            get => selectedListNumderRepeats;
            set
            {
                selectedListNumderRepeats = value;
                Signal();
            }
        }

        public List<ClassCheckad> ListTimeBreak
        {
            get => listTimeBreak;
            set
            {
                listTimeBreak = value;
                Signal();
            }
        }
        public ClassCheckad SelectedListTimeBreak
        {
            get => selectedListTimeBreak;
            set
            {
                selectedListTimeBreak = value;
                Signal();
            }
        }


        public List<string> ListTime
        {
            get => listTime;
            set
            {
                listTime = value;
                Signal();
            }
        }

        public string SelectedListTime { get => selectedListTime;
            set
            {
                selectedListTime = value;
                Signal();
            }
        }

        public СountdownTimer CountdownTimer
        { get => сountdownTimer;
            set
            {
                сountdownTimer = value;
                Signal();
            }
        }

        // Свойство Countdown возвращает строку с правильным форматом минута:секунда
        public string Countdown
        {
            get => GetFormattedTime(_remainingTime);
            set
            {
                _remainingTime = TimeSpan.Parse(value);
                Signal();
            }
        }



        private RelayCommand startCountdownTimer;
        public RelayCommand StartCountdownTimer
        {
            get
            {
                return startCountdownTimer ?? new RelayCommand(async () =>
                {
                    if (CountdownTimer.Timer == null || !CountdownTimer.Timer.IsEnabled)
                    {
                        if (TextHour != null || TextHour != "" || TextMin != null || TextMin != "" || TextSec != null || TextSec != "")
                        {
                            if (TextHour != "00" || TextMin != "00" || TextSec != "00")
                            {
                                СountdownTimer countdownTimer = CountdownTimer;
                                CountdownTimer = new();
                                CountdownTimer.SettingRepeat(countdownTimer.BreakTime, countdownTimer.Repetitions);
                                CountdownTimer.StartTimer(int.Parse(TextHour), int.Parse(TextMin), int.Parse(TextSec));
                                CountdownTimer = CountdownTimer;
                                CountdownTimer.Timer.Start();

                            }
                        }

                    }
                    
                }

                );

            }

        }

        private RelayCommand saveRepeatTimer;
        public RelayCommand SaveRepeatTimer
        {
            get
            {
                return saveRepeatTimer ?? new RelayCommand(async () =>
                {

                    ClassCheckad numderRepeats = ListNumderRepeats.FirstOrDefault(s => s.Check == true);
                    ClassCheckad timeBreak = ListTimeBreak.FirstOrDefault(s => s.Check == true);
                    if (numderRepeats != null && timeBreak != null)
                    {
                        TimeSpan timeSpanBreak = new();
                        switch (SelectedListTime)
                        {
                            case "сек":
                                timeSpanBreak = new(0,0, timeBreak.Value);
                                break;
                            case "мин":
                                timeSpanBreak = new(0, timeBreak.Value, 0);
                                break;
                            case "час":
                                timeSpanBreak = new(timeBreak.Value, 0, 0);
                                break;
                        }

                        CountdownTimer.SettingRepeat(timeSpanBreak, numderRepeats.Value);
                        CountdownTimer = CountdownTimer;
                    }
                    else
                    {
                        //MessageBox.Show("none");
                        return;
                    }

                }

                );

            }

        }







        private RelayCommand startSelectCountdownTimer;
        public RelayCommand StartSelectCountdownTimer
        {
            get
            {
                return startSelectCountdownTimer ?? new RelayCommand(async () =>
                {
                    if (CountdownTimer.Timer == null || !CountdownTimer.Timer.IsEnabled)
                    {
                        if (TextTime != null || TextTime != "")
                        {
                            if (TextTime != "00")
                            {
                                СountdownTimer countdownTimer = CountdownTimer;
                                CountdownTimer = new();
                                CountdownTimer.SettingRepeat(countdownTimer.BreakTime, countdownTimer.Repetitions);
                                switch (SelectedListTime)
                                {
                                    case "сек":
                                        CountdownTimer.StartTimer(0, 0, int.Parse(TextTime));
                                        break;
                                    case "мин":
                                        CountdownTimer.StartTimer(0, int.Parse(TextTime), 0);
                                        break;
                                    case "час":
                                        CountdownTimer.StartTimer(int.Parse(TextTime), 0, 0);
                                        break;
                                }
                                CountdownTimer = CountdownTimer;
                                CountdownTimer.Timer.Start();
                                TextHour = CountdownTimer.SpecifiedTime.Hours.ToString();
                                TextMin = CountdownTimer.SpecifiedTime.Minutes.ToString();
                                TextSec = CountdownTimer.SpecifiedTime.Seconds.ToString();

                            }
                        }

                    }

                }

                );

            }

        }





        private RelayCommand stopCountdownTimer;

        public RelayCommand StopCountdownTimer
        {
            get
            {
                return stopCountdownTimer ?? new RelayCommand(async () =>
                {
                    if (CountdownTimer.Timer != null && CountdownTimer.RemainingTime != new TimeSpan())
                    {

                        CountdownTimer.PauseTimer();



                    }

                }

                );

            }

        }



        public PomodoroTimerControle()
        {
            CountdownTimer = new();
            SpecifiedTime = new();
            TextHour = "00";
            TextMin = "00";
            TextSec = "00";
            ListTime = new()
            {
                "сек",
                "мин",
                "час",
            };
            ListNumderRepeats = new()
            { new(){ Content = "2", Check= false, Group ="Number", Value=2 },
            new(){ Content = "3", Check= false, Group ="Number", Value = 3 },
            new(){ Content = "5", Check= false, Group ="Number", Value = 5 },
            new(){ Content = "Свой вариант", Check= false, Group ="Number" },
            };
            ListTimeBreak = new()
            {
                new(){ Content = "5 минут", Check= false, Group ="Break", Value = 5 },
            new(){ Content = "10 минут", Check= false, Group ="Break", Value = 10 },
            new(){ Content = "15 минут", Check= false, Group ="Break", Value = 15 },
            new(){ Content = "Свой вариант", Check= false, Group ="Break" },
            };
            SelectedListTime = listTime[0];
        }


        // Вспомогательный метод для правильного форматирования времени
        private string GetFormattedTime(TimeSpan time)
        {
            return time.Minutes + ":" + time.Seconds.ToString().PadLeft(2, '0');
            
        }



        internal void SetControl(Pomodoro pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            Pomodoro = pass;
        }


        internal void SetDispatcher(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

    }
}
