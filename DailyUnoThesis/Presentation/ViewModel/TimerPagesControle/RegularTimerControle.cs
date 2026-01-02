
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Timer;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.TimerPagesControle
{
    public class RegularTimerControle:Base
    {

        private RegularTimer Page;
        private CoreDispatcher dispatcher;
        private CountupTimer countupTimer;
        private List<Mission> missions;
        private Mission selectedMission;
        private Mission mission;

        public CountupTimer CountupTimer
        {
            get => countupTimer;
            set
            {
                countupTimer = value;
                Signal();
            }
        }


        public List<Mission> Missions
        {
            get => missions; set
            {
                missions = value;
                Signal();
            }
        }

        public Mission Mission
        {
            get => mission;
            set
            {
                mission = value;
                Signal();
            }
        }

        public Mission SelectedMission
        { get => selectedMission; 
            set 
            {
                selectedMission = value;
                Signal();
            }
        }

        private RelayCommand startCountupTimer;
        public RelayCommand StartCountupTimer
        {
            get
            {
                return startCountupTimer ?? new RelayCommand(async () =>
                {
                    if (CountupTimer.Timer == null || !CountupTimer.Timer.IsEnabled)
                    {

                        CountupTimer countupTimer = CountupTimer;
                        CountupTimer = new();
                        CountupTimer.StartTimer(0, 0, 0);
                        CountupTimer = CountupTimer;
                        CountupTimer.Timer.Start();
                        

                    }

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
                    Missions = new List<Mission>(Missions);
                    SelectedMission = new();
                }
                );
            }
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
            await UpdateLists(missions);
        }

        private async Task UpdateLists(List<Mission> missions)
        {
            List<Mission> submissions = new List<Mission>();
            Missions = new();
            Missions = missions;
            Missions.RemoveAll(x => x.IdUpMission != null);
            await RemoveCompleteTask(Missions);
            //foreach (Mission mis in Missions)
            //{
            //    if (mis.InverseIdUpMissionNavigation.Count != 0)
            //    {

            //    }
            //}
            Missions = new(Missions);
        }

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
