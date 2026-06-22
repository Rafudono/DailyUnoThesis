
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Android.OS;

//using Android.OS;

//using Android.Graphics.Drawables;
//using Android.OS;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Timer;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.TimerPagesControle
{
    public partial class RegularTimerControle: ObservableObject
    {

        private RegularTimer Page;
        private CoreDispatcher dispatcher;

        [ObservableProperty]
        private CountupTimer countupTimer;

        [ObservableProperty]
        //private List<Mission> missions;
        public ObservableCollection<Mission> missions;

        [ObservableProperty]
        private Mission selectedMission;

        [ObservableProperty]
        private Mission mission;

        [ObservableProperty]
        private string nameTimer;

        [ObservableProperty]
        private bool isTiming = false;

        [ObservableProperty]
        private bool isIcon = true;


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
        private string dateNow = DateTime.Today.ToString("d-MMM-yyyy");

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

        partial void OnIsEnoughSpaceChanged(bool args)
        {
            IsPaneOpen = args? true: false;
            IsDarkSpace = false;
        }

        //public CountupTimer CountupTimer
        //{
        //    get => countupTimer;
        //    set
        //    {
        //        countupTimer = value;
        //        Signal();
        //    }
        //}


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
        //{ get => selectedMission; 
        //    set 
        //    {
        //        selectedMission = value;
        //        Signal();
        //    }
        //}

        private RelayCommand startCountupTimer;
        public RelayCommand StartCountupTimer
        {
            get
            {
                return startCountupTimer ?? new RelayCommand(async () =>
                {

                    if (!IsTiming)
                    {
                        if (CountupTimer.Timer == null || !CountupTimer.Timer.IsEnabled)
                        {
                           
                                CountupTimer countupTimer = CountupTimer;
                                CountupTimer = new();
                                CountupTimer.StartTimer(0, 0, 0);
                                IsTiming = true;
                                IsIcon = false;
                                CountupTimer.IsPaused = false;
                                CountupTimer.Restart = false;
                            CountupTimer.StartPlayTimer = DateTime.Now;
                                CountupTimer.Timer.Start();
                            

                        }
                    }
                    else
                    {
                        StopCountupTimer.Execute(null);
                    }

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



        private RelayCommand stopCountupTimer;
        public RelayCommand StopCountupTimer
        {
            get
            {
                return stopCountupTimer ?? new RelayCommand(async () =>
                {
                    if (CountupTimer.Timer != null && CountupTimer.RemainingTime != new TimeSpan())
                    {
                        CountupTimer.PauseTimer();
                        if (CountupTimer.IsPaused)
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

        private RelayCommand restartCountupTimer;
        public RelayCommand RestartCountupTimer
        {
            get
            {
                return restartCountupTimer ?? new RelayCommand(async () =>
                {
                    if (CountupTimer.Timer == null || !CountupTimer.Timer.IsEnabled)
                    {
                        IsTiming = false;
                        StartCountupTimer.Execute(null);
                    }
                }
                );
            }
        }


        private RelayCommand selectedItemTreeViewItem;
        public RelayCommand SelectedItemTreeViewItem
        {
            get
            {
                return selectedItemTreeViewItem ?? new RelayCommand(async () =>
                {
                  //await  this.dispatcher.Invoke(async() =>  //диспатчер не хочет в асинхронность
                  //  {
                  //      object ob = Page.tree.SelectedItem;  //нет tree
                  //      SelectedMission = (Mission)ob;
                  //  });
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
                    //await this.dispatcher.Invoke(async () =>
                    //{
                    //    Page.tree.item
                        
                    //});
                    //Missions = new List<Mission>(Missions);
                    SelectedMission = new();
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
            else if(!IsPaneOpen)
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


        public RegularTimerControle()
        {
            CountupTimer = new CountupTimer();
            FillData();
        }

        public async Task FillData()
        {
            List<Mission> missions = new List<Mission>();
            missions = await APIHost.GetInstance().GetMissions();
            Missions = new();
            Missions.AddRange(missions);
            //GetCategories();
            //await UpdateLists(mission, missions);


        }

        private RelayCommand saveRegularTimer;

        public RelayCommand SaveRegularTimer
        {
            get
            {
                return saveRegularTimer ?? new RelayCommand(async () =>
                {
                    await SaveRegularTimerToUser();
                }
                );
            }
        }
        private async Task SaveRegularTimerToUser()
        {
            if (!CountupTimer.IsPaused && CountupTimer.Timer.IsEnabled)
            {

                var contentDialog = new ContentDialog
                {
                    Title = "Не пауза",
                    //Content = "This is a very important message.",
                    PrimaryButtonText = "OK",
                    XamlRoot = Page.XamlRoot
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
                    XamlRoot = Page.XamlRoot
                };
                await contentDialog.ShowAsync();
                return;

            }

            //var today = DateTime.Now;
            Missionstimer missionstimer = new Missionstimer()
            {
                StartDate = CountupTimer.StartPlayTimer,
                EndDate = CountupTimer.EndTimer,
                DurationTimer = new TimeOnly(CountupTimer.RemainingTime.Hours, CountupTimer.RemainingTime.Minutes, CountupTimer.RemainingTime.Seconds),

            };
            if (SelectedMission == null)
            {
                missionstimer.MissionId = null;
                missionstimer.TitleMission = NameTimer;

            }
            else
            {
                missionstimer.MissionId = SelectedMission.Id;
                missionstimer.TitleMission = SelectedMission.Title;
            }
            await APIHost.GetInstance().CreateMissionTimer(missionstimer);
        }

        //public async Task FillData()
        //{
        //    //List<Mission> missions = new List<Mission>();
        //    //missions = await APIHost.GetInstance().GetMissions();
        //    //await UpdateLists(missions);
        //}

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


        public async void OnItemInvokedTree(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            // args.InvokedItem — это объект задачи или категории, на который кликнули
            var clickedItem = args.InvokedItem as Mission;

            if (clickedItem != null)
            {
                IsSelectedMission = true;
                OpenPanel();
            }

            // Открываем панель подробностей


        }

        internal void SetControl(RegularTimer pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            Page = pass;
        }


        internal void SetDispatcher(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
    }
}
