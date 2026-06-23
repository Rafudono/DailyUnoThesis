using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Timer;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Windows.UI.Core;
using Timer = System.Timers.Timer;

namespace DailyUnoThesis.Presentation.ViewModel.TimerPagesControle
{
    public partial class PomodoroTimerControle: ObservableObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [ObservableProperty]
        private СountdownTimer countdownTimer;

        [ObservableProperty]
        private int textSec;

        [ObservableProperty]
        private int textMin;


        [ObservableProperty]
        private int textHour;

        partial void OnTextSecChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnTextMinChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnTextHourChanged(int args)
        {
            WarningOfRestrictions();
        }

        [ObservableProperty]
        private int textTime = 25;

        [ObservableProperty]
        private int shortBreak = 5;

        [ObservableProperty]
        private int bigBreak = 20;

        [ObservableProperty]
        private int tomatoesInRound = 3;

        [ObservableProperty]
        private int numberOfTomatoes = 6;

        [ObservableProperty]
        private int numberOfRounds = 2;

        partial void OnTextTimeChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnShortBreakChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnBigBreakChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnTomatoesInRoundChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnNumberOfTomatoesChanged(int args)
        {
            WarningOfRestrictions();
        }
        partial void OnNumberOfRoundsChanged(int args)
        {

        }

        private void WarningOfRestrictions()
        {
            if (ModeHmsTime)
            {
                if (TextTime < 10)
                {
                    Warning = true;
                    return;
                }
               
            }
            else 
            {
                if (TextSec == 0 && TextMin == 0 && TextHour == 0)
                {
                    Warning = true;
                    SecondNullTimer = true;
                    return;
                }
                else
                    SecondNullTimer = false;
                
            }
            if ( ShortBreak < 3 || BigBreak < 15 || TomatoesInRound < 1 || NumberOfTomatoes < 1 || NumberOfRounds < 1)
            {
                Warning = true;
                return;
            }
           
            //else if (Warning == true)
            //{
                Warning = false;
            
            //}
        }

        partial void OnSelectedTimerChanged(Focustimer args)
        {
            if (args != null && args.Id != 0)
            {
                IsSample = true;

                if (args.DurationTime.Hour != 0 && args.DurationTime.Second != 0)
                {
                    TextSec = args.DurationTime.Second;
                    TextMin = args.DurationTime.Minute;
                    TextHour = args.DurationTime.Hour;
                    ModeHmsTime = false;
                    ModeSettingTime = true;

                }
                else
                {
                    ModeHmsTime = true;
                    ModeSettingTime = false;
                }
                TextTime = args.DurationTime.Minute;
                if (args.DurationBreak != null)
                {
                    ShortBreak = args.DurationBreak.Value.Minute;
                    BigBreak = args.LongBreakTime.Value.Minute;
                    TomatoesInRound = (int)args.Repetitions;
                    if ((bool)args.IsRound)
                    {
                        NumberOfRounds = (int)args.CountRound;
                    }
                    else
                    {
                        NumberOfTomatoes = (int)args.CountRound;

                    }
                }
                if (args.MissionId != null && args.MissionId != 0)
                {
                    NameTimer = args.Mission.Title;
                    IsSelectedMission = true;
                }
                else
                {
                    NameTimer = args.TitleMission;
                    IsSelectedMission = false;
                }

            }
            else if (IsSample == true)
            {
                IsSample = false;
            }
        }

        partial void OnSelectedMissionChanged(Mission args)
        {
            if (args != null && args.Id != 0)
            {
                NameTimer = args.Title;
                IsSelectedMission = true;
            }
            else
            {
                IsSelectedMission = false;
            }
        }

        

        [ObservableProperty]
        private TimeSpan timeSpan;

        [ObservableProperty]
        private Pomodoro pomodoro;

        [ObservableProperty]
        private Dispatcher dispatcher;

        [ObservableProperty]
        private TimeOnly specifiedTime;

        [ObservableProperty]
        private Pomodoro taskPomodoro;

        [ObservableProperty]
        private List<string> listTime;

        [ObservableProperty]
        private List<ClassCheckad> listNumderRepeats;

        [ObservableProperty]
        private string selectedListTime;

        [ObservableProperty]
        private string nameTimer;

        [ObservableProperty]
        private TimeSpan _remainingTime;

        [ObservableProperty]
        private ClassCheckad selectedListNumderRepeats;

        [ObservableProperty]
        private List<ClassCheckad> listTimeBreak;

        [ObservableProperty]
        private ClassCheckad selectedListTimeBreak;

        //[ObservableProperty]
        public ObservableCollection<Mission> Missions => ViewModelStore.GetInstance().AllTasks.Missions;

        [ObservableProperty]
        private Mission selectedMission;

        [ObservableProperty]
        private Mission mission;

        [ObservableProperty]
        private List<Focustimer> missionsTimers;

        [ObservableProperty]
        private Focustimer selectedTimer;

        [ObservableProperty]
        private bool modeHmsTime = true;
        [ObservableProperty]
        private bool modeSettingTime = false;

        [ObservableProperty]
        private bool isRepetitions = true;

        [ObservableProperty]
        private bool modeTomatosOrRound =true;

        [ObservableProperty]
        private bool isTiming = false;

        [ObservableProperty]
        private bool isIcon = true;

        [ObservableProperty]
        private bool forward = true;

        [ObservableProperty]
        private bool backward = true;

        [ObservableProperty]
        private bool warning = false;
        [ObservableProperty]
        private bool secondNullTimer = true;
        [ObservableProperty]
        private bool isPaneOpen = false;
        [ObservableProperty]
        private bool isOverlay = true;
        [ObservableProperty]
        private bool isEnoughSpace = false;
        [ObservableProperty]
        private bool isDarkSpace = false;
        [ObservableProperty]
        private bool isSample = false;
        [ObservableProperty]
        private bool isSelectedMission = false;
        [ObservableProperty]
        private bool isMissions = false;


        [ObservableProperty]
        private bool isAbsoluteEnd = true;
        [ObservableProperty]
        private bool isStopForBreak = true;
        

        [ObservableProperty]
        private bool isDoubleTimer = false;

        partial void OnIsEnoughSpaceChanged(bool args)
        {
            IsPaneOpen = args ? true : false;
            IsDarkSpace = false;
        }


        //public List<Mission> Missions
        //{
        //    get => missions; set
        //    {
        //        missions = value;
        //        Signal();
        //    }
        //}

        //public Mission Mission
        //{
        //    get => mission;
        //    set
        //    {
        //        mission = value;
        //        Signal();
        //    }
        //}

        //public Mission SelectedMission
        //{
        //    get => selectedMission;
        //    set
        //    {
        //        selectedMission = value;
        //        Signal();
        //    }
        //}



        //public TimeOnly SpecifiedTime
        //{
        //    get => specifiedTime;
        //    set
        //    {
        //        specifiedTime = value;
        //        Signal();
        //    }
        //}
        //public TimeSpan TimeSpan { get => timeSpan; set { timeSpan = value; Signal(); } }

        //public string TextHour { get => textHour; set { textHour = value; Signal(); } }
        //public string TextMin { get => textMin; set { textMin = value; Signal(); } }
        //public string TextSec { get => textSec; set { textSec = value; Signal(); } }
        //public string TextTime { get => textTime; set { textTime = value; Signal(); } }

        //public List<ClassCheckad> ListNumderRepeats
        //{
        //    get => listNumderRepeats;
        //    set
        //    {
        //        listNumderRepeats = value;
        //        Signal();
        //    }
        //}
        //public ClassCheckad SelectedListNumderRepeats
        //{
        //    get => selectedListNumderRepeats;
        //    set
        //    {
        //        selectedListNumderRepeats = value;
        //        Signal();
        //    }
        //}

        //public List<ClassCheckad> ListTimeBreak
        //{
        //    get => listTimeBreak;
        //    set
        //    {
        //        listTimeBreak = value;
        //        Signal();
        //    }
        //}
        //public ClassCheckad SelectedListTimeBreak
        //{
        //    get => selectedListTimeBreak;
        //    set
        //    {
        //        selectedListTimeBreak = value;
        //        Signal();
        //    }
        //}


        //public List<string> ListTime
        //{
        //    get => listTime;
        //    set
        //    {
        //        listTime = value;
        //        Signal();
        //    }
        //}

        //public string SelectedListTime
        //{
        //    get => selectedListTime;
        //    set
        //    {
        //        selectedListTime = value;
        //        Signal();
        //    }
        //}

        //public СountdownTimer CountdownTimer
        //{
        //    get => сountdownTimer;
        //    set
        //    {
        //        сountdownTimer = value;
        //        Signal();
        //    }
        //}

        // Свойство Countdown возвращает строку с правильным форматом минута:секунда
        //public string Countdown
        //{
        //    get => GetFormattedTime(_remainingTime);
        //    set
        //    {
        //        _remainingTime = TimeSpan.Parse(value);
        //        Signal();
        //    }
        //}

        //public List<Focustimer> MissionsTimers
        //{
        //    get => missionsTimers; set
        //    {
        //        missionsTimers = value;
        //        Signal();
        //    }
        //}

        //public Focustimer SelectedTimer
        //{
        //    get => selectedTimer;
        //    set
        //    {
        //        selectedTimer = value;
        //        Signal();
        //    }
        //}

        private RelayCommand openPanelSample;
        public RelayCommand OpenPanelSample
        {
            get
            {
                return openPanelSample ?? new RelayCommand(async () =>
                {
                    IsMissions = false;
                    OpenPanel();


                }
                );
            } 
        }

        private RelayCommand openPanelTree;
        public RelayCommand OpenPanelTree
        {
            get
            {
                return openPanelTree ?? new RelayCommand(async () =>
                {

                    IsMissions = true;
                    OpenPanel();

                }
                );
            }
        }

        private void OpenPanel()
        {
            if (!IsEnoughSpace)
            {
                IsPaneOpen = IsPaneOpen ? false : true;
            }
            else if (!IsPaneOpen)
            {
                IsPaneOpen = true;
            }
            if (IsEnoughSpace && IsPaneOpen)
            {
                IsOverlay = false;
            }
            else
                IsOverlay = true;
            if (!IsEnoughSpace && IsPaneOpen)
                IsDarkSpace = true;
            else
                IsDarkSpace = false;
        }

        public void ClosePanel()
        {
            OpenPanel();
        }

        private RelayCommand startCountdownTimer;
        public RelayCommand StartCountdownTimer
        {
            get
            {
                return startCountdownTimer ?? new RelayCommand(async () =>
                {

                    if (!IsTiming || CountdownTimer.IsEnd)
                    {
                        if (CountdownTimer.Timer == null || !CountdownTimer.Timer.IsEnabled)
                        {
                            if (CreateCountdownTimer())
                            {
                                IsTiming = true;
                                IsIcon = false;

                                CountdownTimer.IsPaused = false;
                                CountdownTimer.Restart = false;
                                CountdownTimer.IsEnd = false;
                                CountdownTimer.Timer.Start();
                            }

                        }
                    }
                    else
                    {
                        StopCountdownTimer.Execute(null);
                    }
                }
                );
            }
        }


        private RelayCommand restartCountdownTimer;
        public RelayCommand RestartCountdownTimer
        {
            get
            {
                return restartCountdownTimer ?? new RelayCommand(async () =>
                {
                    if (CountdownTimer.Timer == null || !CountdownTimer.Timer.IsEnabled)
                    {
                        IsTiming = false;
                        StartCountdownTimer.Execute(null);
                    }
                }
                );
            }
        }


        private RelayCommand startDoubleTimer;
        public RelayCommand StartDoubleTimer
        {
            get
            {
                return startDoubleTimer ?? new RelayCommand(async () =>
                {
                    if ((CountdownTimer.Timer == null || !CountdownTimer.Timer.IsEnabled) &&
                    (ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer == null || !ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.IsEnabled))
                    {
                        CountdownTimer.WithTheSecondTimer = true;
                        CountdownTimer.AbsoluteEnd = IsAbsoluteEnd;
                        CountdownTimer.StopForBreak = IsStopForBreak;
                        IsDoubleTimer = true;
                        ViewModelStore.GetInstance().Main.IsMenuTimer = true;
                        ViewModelStore.GetInstance().Main.IsPause = false;
                        ViewModelStore.GetInstance().RegularTimer.RestartCountupTimer.Execute(null);
                        RestartCountdownTimer.Execute(null);

                    }
                }
                );
            }
        }



        public bool CreateCountdownTimer()
        {
            bool isОk = false;
            СountdownTimer countdownTimer = CountdownTimer;
            CountdownTimer = new() { WithTheSecondTimer = countdownTimer.WithTheSecondTimer, AbsoluteEnd = countdownTimer.AbsoluteEnd, StopForBreak = countdownTimer.StopForBreak };
            if (IsRepetitions)
            {
                if (ShortBreak < 3)
                {
                    ShortBreak = 5;
                }
                if (BigBreak < 15)
                {
                    BigBreak = 15;
                }
                if (TomatoesInRound < 1)
                {
                    TomatoesInRound = 1;
                }
                if (NumberOfTomatoes < 1)
                {
                    NumberOfTomatoes = 1;
                }
                if (NumberOfRounds < 1)
                {
                    NumberOfRounds = 1;
                }

                CountdownTimer.SettingRepeat(new TimeSpan(0, ShortBreak, 0), new TimeSpan(0, BigBreak, 0), TomatoesInRound, NumberOfTomatoes, NumberOfRounds, ModeTomatosOrRound);
               
                    //ShortBreak += delta;
                  
                    //BigBreak += delta;
                 
                    //TomatoesInRound += delta;
                 
                    //NumberOfTomatoes += delta;
                   
                    //NumberOfRounds += delta;

                }
            //var selectedTab = Pomodoro.SwichTimers.SelectedItem as TabItem;

            if (ModeHmsTime != null)
            {
                if (!ModeHmsTime)
                {
                    //if (TextHour != null || TextHour != "" || TextMin != null || TextMin != "" || TextSec != null || TextSec != "")
                    //{
                    if (TextHour != 0 || TextMin != 0 || TextSec != 0)
                    {
                        //СountdownTimer countdownTimer = CountdownTimer;
                        //CountdownTimer = new();
                        //CountdownTimer.SettingRepeat(countdownTimer.BreakTime, countdownTimer.Repetitions);
                        //if (TextSec > 59)
                        //    TextSec = 59;

                        CountdownTimer.StartTimer(/*int.Parse(*/TextHour, /*int.Parse(*/TextMin, /*int.Parse(*/TextSec);
                            isОk = true;

                        //CountdownTimer.Timer.Start();

                    }
                    //}
                }
                else
                {
                    //if (TextTime != null || TextTime != "")
                    //{
                    if (TextTime != 0)
                    {
                        //СountdownTimer countdownTimer = CountdownTimer;
                        //CountdownTimer = new();
                        //CountdownTimer.SettingRepeat(countdownTimer.BreakTime, countdownTimer.Repetitions);
                        if(TextTime < 10)
                        { 
                            TextTime = 10;
                        }
                        CountdownTimer.StartTimer(0, TextTime, 0);

                        //switch (SelectedListTime)
                        //    {
                        //        case "сек":
                        //            CountdownTimer.StartTimer(0, 0, int.Parse(TextTime));
                        //            break;
                        //        case "мин":
                        //            CountdownTimer.StartTimer(0, int.Parse(TextTime), 0);
                        //            break;
                        //        case "час":
                        //            CountdownTimer.StartTimer(int.Parse(TextTime), 0, 0);
                        //            break;
                        //    }
                        //CountdownTimer = CountdownTimer;
                        //CountdownTimer.Timer.Start();
                        //TextHour = CountdownTimer.SpecifiedTime.Hours /*.ToString()*/;
                        //TextMin = CountdownTimer.SpecifiedTime.Minutes/*.ToString()*/;
                        //TextSec = CountdownTimer.SpecifiedTime.Seconds/*.ToString()*/;
                        isОk = true;

                    }
                    //}
                }
            }
           
            CountdownTimer = CountdownTimer;
            TextHour = CountdownTimer.SpecifiedTime.Hours /*.ToString()*/;
            TextMin = CountdownTimer.SpecifiedTime.Minutes/*.ToString()*/;
            TextSec = CountdownTimer.SpecifiedTime.Seconds/*.ToString()*/;
            return isОk;
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


                    }

                }

                );

            }

        }


        private RelayCommand<string> _adjustTimeCommand;
        public RelayCommand<string> AdjustTimeCommand => _adjustTimeCommand ??= new RelayCommand<string>(param =>
        {
            if (string.IsNullOrEmpty(param)) return;

            // Разделяем параметр по двоеточию
            var parts = param.Split(':');
            if (parts.Length != 2) return;

            string target = parts[0]; // Название: Timer, Short или Big
            int delta = int.Parse(parts[1]); // Значение: 1 или -1

            switch (target)
            {
                case "Timer":
                    TextTime += delta;
                    break;
                case "Short":
                    ShortBreak += delta;
                    break;
                case "Big":
                    BigBreak += delta;
                    break;
                case "TomatRount":
                    TomatoesInRound += delta;
                    break;
                case "Tomato":
                    NumberOfTomatoes += delta;
                    break;
                case "Round":
                    NumberOfRounds += delta;
                    break;
            }
        });

        private RelayCommand<string> arrowControlTimer;
        public RelayCommand<string> ArrowControlTimer
        {
            get
            {
                return arrowControlTimer ?? new RelayCommand<string>(async (typeOfArrows) =>
                {
                    switch (typeOfArrows)
                    {
                        case "forward":
                            TextTime += 1;
                            break;
                        case "backward":
                            TextTime -= 1;
                            break;
                    }
                    
                }
                );
            }
        }

        private RelayCommand<string> arrowControlShortBreak;
        public RelayCommand<string> ArrowControlShortBreak
        {
            get
            {
                return arrowControlShortBreak ?? new RelayCommand<string>(async (typeOfArrows) =>
                {
                    switch (typeOfArrows)
                    {
                        case "forward":
                            ShortBreak += 1;
                            break;
                        case "backward":
                            ShortBreak -= 1;
                            break;
                    }
                }
                );
            }
        }

        private RelayCommand<string> arrowControlBigBreak;
        public RelayCommand<string> ArrowControlBigBreak
        {
            get
            {
                return arrowControlBigBreak ?? new RelayCommand<string>(async (typeOfArrows) =>
                {
                    switch (typeOfArrows)
                    {
                        case "forward":
                            BigBreak += 1;
                            break;
                        case "backward":
                            BigBreak -= 1;
                            break;
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
                                timeSpanBreak = new(0, 0, timeBreak.Value);
                                break;
                            case "мин":
                                timeSpanBreak = new(0, timeBreak.Value, 0);
                                break;
                            case "час":
                                timeSpanBreak = new(timeBreak.Value, 0, 0);
                                break;
                        }

                        //CountdownTimer.SettingRepeat(timeSpanBreak, numderRepeats.Value);
                        CountdownTimer = CountdownTimer;
                        //await this.dispatcher.Invoke(async () =>
                        //{
                        //    Pomodoro.RepeatPanel.Visibility = Visibility.Collapsed;
                        //});

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
                        if (CountdownTimer.IsPaused)
                        {
                            IsIcon = true;
                        }
                        else
                        {
                            IsIcon = false;
                        }

                    }
                }

                );

            }

        }


        private RelayCommand delelteReapits;

        public RelayCommand DelelteReapits
        {
            get
            {
                return delelteReapits ?? new RelayCommand(async () =>
                {
                    DelelteReapitsCheck();
                    CountdownTimer = CountdownTimer;
                }

                );

            }

        }

        private void DelelteReapitsCheck()
        {
            if (CountdownTimer.Timer == null)
            {
                //CountdownTimer.SettingRepeat(new TimeSpan(), 0);
                return;
            }
            if (!CountdownTimer.IsPaused && CountdownTimer.Timer.IsEnabled)
            {
                return;
            }
            //CountdownTimer.SettingRepeat(new TimeSpan(), 0);
        }


        private RelayCommand savePomodoroTimer;

        public RelayCommand SavePomodoroTimer
        {
            get
            {
                return savePomodoroTimer ?? new RelayCommand(async () =>
                {
                    if (SelectedTimer != null)
                    {
                        SelectedTimer.Id = 0;
                    }
                    await SavePomodoroTimerCheck();
                    CountdownTimer = CountdownTimer;
                }

                );

            }

        }

        private async Task SavePomodoroTimerCheck()
        {
            if (!CreateCountdownTimer())
            {
                var contentDialog = new ContentDialog
                {
                    Title = "Заполните все поля таймера",
                    //Content = "This is a very important message.",
                    PrimaryButtonText = "OK",
                    XamlRoot = Pomodoro.XamlRoot
                };
                await contentDialog.ShowAsync();
                return;
            }
            if (!CountdownTimer.IsPaused && CountdownTimer.Timer.IsEnabled)
            {

                var contentDialog = new ContentDialog
                {
                    Title = "Не пауза",
                    //Content = "This is a very important message.",
                    PrimaryButtonText = "OK",
                    XamlRoot = Pomodoro.XamlRoot
                };
                await contentDialog.ShowAsync();
                return;
            }
            if (Warning)
            {
                var contentDialog = new ContentDialog
                {
                    Title = "Заполните все поля",
                    //Content = "This is a very important message.",
                    PrimaryButtonText = "OK",
                    XamlRoot = Pomodoro.XamlRoot
                };
                await contentDialog.ShowAsync();
                return;
            }

            if ((NameTimer == "" || NameTimer == null))
            {
                var contentDialog = new ContentDialog
                {
                    Title = "Заполните название шаблона",
                    //Content = "This is a very important message.",
                    PrimaryButtonText = "OK",
                    XamlRoot = Pomodoro.XamlRoot
                };
                await contentDialog.ShowAsync();
                return;

            }

            //var today = DateTime.Now;
            Focustimer focustimer = new Focustimer()
            {
                DurationTime = new TimeOnly(CountdownTimer.SpecifiedTime.Hours, CountdownTimer.SpecifiedTime.Minutes, CountdownTimer.SpecifiedTime.Seconds),
            };
            if (IsRepetitions)
            {
                focustimer.Repetitions = TomatoesInRound;
                if (ModeTomatosOrRound)
                {
                    focustimer.CountRound = NumberOfRounds;
                    focustimer.IsRound = true;
                }
                else
                {
                    focustimer.CountRound = NumberOfTomatoes;
                    focustimer.IsRound = false;
                }
                focustimer.DurationBreak = new TimeOnly(CountdownTimer.BreakTime.Hours, CountdownTimer.BreakTime.Minutes, CountdownTimer.BreakTime.Seconds);
                focustimer.LongBreakTime = new TimeOnly(CountdownTimer.LongBreakTime.Hours, CountdownTimer.LongBreakTime.Minutes, CountdownTimer.LongBreakTime.Seconds);
            }
            if (SelectedMission == null || SelectedMission.Id == 0)
            {
                focustimer.MissionId = null;
                focustimer.TitleMission = NameTimer;
                //focustimer.DescriptionMission = Mission.Description;
            }
            else
            {
                focustimer.MissionId = SelectedMission.Id;
                focustimer.TitleMission = SelectedMission.Title;
                //focustimer.DescriptionMission = SelectedMission.Description;
            }
            if (SelectedTimer is null || SelectedTimer.Id == 0)
            {
                await APIHost.GetInstance().CreateFocustimer(focustimer);
            }
            else
            {
                focustimer.Id = SelectedTimer.Id;
                //focustimer.MissionId = SelectedTimer.MissionId;
                focustimer.UserId = SelectedTimer.UserId;
                //await APIHost.GetInstance().EditFocustimer(focustimer);
                await APIHost.GetInstance().EditFocustimer(focustimer);

            }
            FillData();
            //MissionsTimers = await APIHost.GetInstance().GetMyFocusTimer();
            //MissionsTimers = new(MissionsTimers);
        }



        private RelayCommand selectedItemTreeViewItem;
        public RelayCommand SelectedItemTreeViewItem
        {
            get
            {
                return selectedItemTreeViewItem ?? new RelayCommand(async () =>
                {
                    //await this.dispatcher.Invoke(async () =>
                    //{
                    //    object ob = Pomodoro.tree.SelectedItem;
                    //    SelectedMission = (Mission)ob;
                    //});
                }
                );
            }
        }


        private RelayCommand nullItemTreeViewItem;
        public RelayCommand NullItemTreeViewItem
        {
            get
            {
                return nullItemTreeViewItem ?? new RelayCommand(async () =>
                {
                    //Missions = new List<Mission>(Missions);
                    SelectedMission = new();
                }
                );
            }
        }


        private RelayCommand selectedTimerInList;
        public RelayCommand SelectedTimerInList
        {
            get
            {
                return selectedTimerInList ?? new RelayCommand(async () =>
                {
                    if (SelectedTimer != null)
                    {
                        TextHour = SelectedTimer.DurationTime.Hour /*.ToString()*/;
                        TextMin = SelectedTimer.DurationTime.Minute/*.ToString()*/;
                        TextSec = SelectedTimer.DurationTime.Second/*.ToString()*/;
                        if (SelectedTimer.MissionId != null /*|| SelectedTimer.MissionId != 0*/)
                            SelectedMission = SelectedTimer.Mission;
                        else if (SelectedTimer.TitleMission != null)
                        {
                            Mission = new() { Title = SelectedTimer.TitleMission};
                        }
                        if (SelectedTimer.Repetitions != null)
                        {
                            CountdownTimer.Repetitions = (int)SelectedTimer.Repetitions;
                            TimeOnly timeOnly = (TimeOnly)SelectedTimer.DurationBreak;
                            CountdownTimer.BreakTime = new TimeSpan(timeOnly.Hour, timeOnly.Minute, timeOnly.Second);
                        }
                        CountdownTimer = CountdownTimer;
                    }
                }
                );
            }
        }
        private RelayCommand clearSelectedTimerInList;
        public RelayCommand ClearSelectedTimerInList
        {
            get
            {
                return clearSelectedTimerInList ?? new RelayCommand(async () =>
                {
                    //TextHour = "00";
                    //TextMin = "00";
                    //TextSec = "00";
                    //TextTime = "00";
                    Mission = new();
                    //Pomodoro.Timers.SelectedItem = null;
                    CountdownTimer = new();
                    SelectedTimer = new();
                    CountdownTimer = CountdownTimer;
                    NullItemTreeViewItem.Execute(null);
                }
                );
            }
        }

        private RelayCommand checkSavePomodoroTimer;
        public RelayCommand CheckSavePomodoroTimer
        {
            get
            {
                return checkSavePomodoroTimer ?? new RelayCommand(async () =>
                {
                    //if (MessageBox.Show("Для сохранения времени таймера нужно запустить и поставить на паузу таймер. Продолжить?",
                    //"Save file",
                    //MessageBoxButton.YesNo,
                    //MessageBoxImage.Question) == MessageBoxResult.Yes)
                    //{
                    //    // Do something here

                    //    if (SelectedTimer != null)
                    //    {

                    //        Focustimer focustimer = MissionsTimers.FirstOrDefault(s => s.Id == SelectedTimer.Id);
                    //        //if (focustimer != SelectedTimer)
                    //        //{
                    //        await this.dispatcher.Invoke(async () =>
                    //        {
                    //            Pomodoro.CheckSaveTimer.Visibility = Visibility.Visible;
                    //        });
                    //        //}

                    //    }
                    //    else
                    //var contentDialog = new ContentDialog()
                    //{
                    //    Content = "Hello world!",
                    //    PrimaryButtonText = "OK",
                    //    XamlRoot = Pomodoro.XamlRoot
                    //};
                    if (SelectedTimer != null && SelectedTimer.Id != 0)
                    {
                        var buttonStack = new StackPanel { Spacing = 10, Margin = new Thickness(0, 20, 0, 0) };

                        var dialog = new ContentDialog()
                        {
                            Title = "СОХРАНЕНИЕ ШАБЛОНА",
                            // Основной текст + кнопки помещаем в Content
                            Content = new StackPanel
                            {
                                Children = {
                new TextBlock {
                    Text = "Вы хотите перезаписать существующий шаблон или создать новый на его основе?",
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Segoe Print"),
                    TextAlignment = TextAlignment.Center,

                },
                buttonStack
            }
                            },
                            XamlRoot = Pomodoro.XamlRoot
                        };

                        // Свой результат для отслеживания выбора
                        ContentDialogResult customResult = ContentDialogResult.None;

                        // Кнопка 1: ЗАМЕНИТЬ
                        var btnReplace = new Button
                        {
                            Content = "ЗАМЕНИТЬ СТАРЫЙ",
                            Style = (Style)Application.Current.Resources["NoirDialogButtonStyle"], // Используем стиль из Варианта 1
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        };
                        btnReplace.Click += (s, e) => { customResult = ContentDialogResult.Primary; dialog.Hide(); };

                        // Кнопка 2: СОЗДАТЬ КОПИЮ
                        var btnCopy = new Button
                        {
                            Content = "СОЗДАТЬ НОВЫЙ",

                            Style = (Style)Application.Current.Resources["NoirDialogButtonStyle"],
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        };
                        btnCopy.Click += (s, e) => { customResult = ContentDialogResult.Secondary; dialog.Hide(); };

                        // Кнопка 3: ОТМЕНА
                        var btnCancel = new Button
                        {
                            Content = "ОТМЕНА",
                            Style = (Style)Application.Current.Resources["NoirDialogButtonStyle"],
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            //Opacity = 0.6 // Сделаем чуть бледнее
                        };
                        btnCancel.Click += (s, e) => { customResult = ContentDialogResult.None; dialog.Hide(); };

                        buttonStack.Children.Add(btnReplace);
                        buttonStack.Children.Add(btnCopy);
                        buttonStack.Children.Add(btnCancel);

                      
                       /* ContentDialogResult result =*/ await dialog.ShowAsync();

                        //if (result == ContentDialogResult.Primary)
                        //{
                        //    // Логика замены старого шаблона
                        //    System.Diagnostics.Debug.WriteLine("Выбрано: Заменить");
                        //}
                        //else if (result == ContentDialogResult.Secondary)
                        //{
                        //    // Логика создания нового шаблона
                        //    System.Diagnostics.Debug.WriteLine("Выбрано: Создать новый");
                        //    SelectedTimer.Id = 0;
                        //}
                        //else
                        //{
                        //    // Нажата кнопка "Отмена" или диалог закрыт клавишей Esc
                        //    System.Diagnostics.Debug.WriteLine("Выбрано: Отмена");
                        //    return;
                        //}

                        // ПРОВЕРЯЕМ ВАШУ ПЕРЕМЕННУЮ customResult
                        if (customResult == ContentDialogResult.Primary)
                        {
                            // Логика замены старого шаблона
                            System.Diagnostics.Debug.WriteLine("Выбрано: Заменить");
                            // Ваш код здесь...
                        }
                        else if (customResult == ContentDialogResult.Secondary)
                        {
                            // Логика создания нового шаблона
                            System.Diagnostics.Debug.WriteLine("Выбрано: Создать новый");
                            SelectedTimer.Id = 0;
                            // Ваш код здесь...
                        }
                        else
                        {
                            // Нажата кнопка "Отмена" или диалог закрыт клавишей Esc
                            System.Diagnostics.Debug.WriteLine("Выбрано: Отмена");
                            return;
                        }
                        //}

                    }
                    await SavePomodoroTimerCheck();

                }
                );
            }
        }

        private RelayCommand editPomodoroTimer;
        public RelayCommand EditPomodoroTimer
        {
            get
            {
                return editPomodoroTimer ?? new RelayCommand(async () =>
                {
                    await SavePomodoroTimerCheck();
                    CountdownTimer = CountdownTimer;
                }
                );
            }
        }

        private RelayCommand<Focustimer> removeFocusTimerCommand;
        public RelayCommand<Focustimer> RemoveFocusTimerCommand
        {
            get
            {
                return removeFocusTimerCommand ?? new RelayCommand<Focustimer>(async (Focustimer) =>
                {
                    await APIHost.GetInstance().DeleteTimer(Focustimer.Id);
                    FillData();
                    //MissionsTimers = await APIHost.GetInstance().GetMyFocusTimer();
                }
                );
            }
        }
        //public RelayCommand RemoveFocusTimerCommand { get; }

        private RelayCommand startBothTimers;
        public RelayCommand StartBothTimers
        {
            get
            {
                return startBothTimers ?? new RelayCommand(async () =>
                {
                    //PageNavigation.GetInstance().StartBothTimers();
                }
                );
            }
        }


        private RelayCommand clearSample;
        public RelayCommand ClearSample
        {
            get
            {
                return clearSample ?? new RelayCommand(async () =>
                {
                    //if (parameter is ListView list)
                    //{
                    //    list.SelectedItem = null;
                    //    list.SelectedIndex = -1;
                    //    list.SelectedItem = 0;
                    //}
                    SelectedTimer = null;

                    TextTime = 25;
                    ShortBreak = 5;
                    BigBreak = 20;
                    TomatoesInRound = 3;
                    NumberOfTomatoes = 6;
                    NumberOfRounds = 2;
                    TextSec = 0;
                    TextMin = 25;
                    TextHour = 0;

                }
                );
            }
        }

        private RelayCommand clearMission;
        public RelayCommand ClearMission
        {
            get
            {
                return clearMission ?? new RelayCommand(async () =>
                {
                    SelectedMission = null;
                    NameTimer = null;

                }
                );
            }
        }

        public PomodoroTimerControle()
        {
            CountdownTimer = new();
            SpecifiedTime = new();
            //TextHour = "00";
            //TextMin = "00";
            //TextSec = "00";
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
            Mission = new Mission();
            SelectedMission = new();
            //RemoveFocusTimerCommand = new RelayCommand<Focustimer>(RemoveFocusTimerAsync);
            FillData();
        }

        public async Task FillData()
        {
            //List<Mission> missions = new List<Mission>();
            //missions = await APIHost.GetInstance().GetMissions();
            MissionsTimers = await APIHost.GetInstance().GetMyFocusTimer();
            //await UpdateLists(missions);
        }

        //private async Task UpdateLists(List<Mission> missions)
        //{
        //    List<Mission> submissions = new List<Mission>();
        //    Missions = new();
        //    Missions = missions;
        //    Missions.RemoveAll(x => x.IdUpMission != null);
        //    await RemoveCompleteTask(Missions);
        //    //foreach (Mission mis in Missions)
        //    //{
        //    //    if (mis.InverseIdUpMissionNavigation.Count != 0)
        //    //    {

        //    //    }
        //    //}
        //    Missions = new(Missions);
        //}

        private async Task RemoveCompleteTask(List<Mission> missions)
        {
            //List<Mission> submissions = new List<Mission>();
            missions.RemoveAll(s => s.IsComplete == true);
            foreach (Mission mission in missions)
            {
                if (mission.InverseIdUpMissionNavigation.Count != 0)
                {
                    await RemoveCompleteTask((List<Mission>)mission.InverseIdUpMissionNavigation);

                }


            }
        }
        
        // Вспомогательный метод для правильного форматирования времени
        private string GetFormattedTime(TimeSpan time)
        {
            return time.Minutes + ":" + time.Seconds.ToString().PadLeft(2, '0');

        }



        public void OnItemInvoked(object sender, ItemClickEventArgs e)
        {
            // e.ClickedItem — это ваш объект из коллекции (например, модель шаблона таймера)
            var clickedItem = e.ClickedItem as Focustimer;

            if (clickedItem != null)
            {
                var time = SelectedTimer;
                OpenPanel();
            }
        }

        public async void OnItemInvokedTree(object sender, ItemClickEventArgs e)
        {
            // args.InvokedItem — это объект задачи или категории, на который кликнули
            var clickedItem = e.ClickedItem as Mission;

            if (clickedItem != null)
            {
                IsSelectedMission = true;
                OpenPanel();
            }
           
            // Открываем панель подробностей


        }



        internal void SetControl(Pomodoro pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            Pomodoro = pass;
        }


        internal void SetDispatcher(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

    }
}
