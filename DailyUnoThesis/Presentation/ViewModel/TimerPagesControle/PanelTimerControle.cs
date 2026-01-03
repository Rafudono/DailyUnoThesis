
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyUnoThesis.Presentation.View.Timer;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.TimerPagesControle
{
    public class PanelTimerControle:Base
    {
        private CoreDispatcher dispatcher { get; set; }
        //public PageNavigation Navigation;
        private PanelTimer TaskPages;
        private Pomodoro PomodoroPage;
        private RegularTimer RegularTimerPage;


        private Page curPageTimer { get; set; }
        public Page CurPageTimer
        {
            get => curPageTimer;
            set
            {
                curPageTimer = value;
                Signal();
            }
        }

        private RelayCommand openPomodoroPage;
        public RelayCommand OpenPomodoroPage
        {
            get
            {
                return openPomodoroPage ?? new RelayCommand(async () =>
                {
                    await GetPomodoroPage();

                }

                );

            }

        }


        private RelayCommand openRegularTimerPage;
        public RelayCommand OpenRegularTimerPage
        {
            get
            {
                return openRegularTimerPage ?? new RelayCommand(async () =>
                {
                    await GetRegularTimerPage();

                }

                );

            }

        }


        public PanelTimerControle()
        {
        }

        public void GetTimers()
        {
            PomodoroPage = new Pomodoro();
            RegularTimerPage = new RegularTimer();

        }


        public async Task GetPomodoroPage()
        {
              this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageTimer = PomodoroPage;
                
            });

        }
        public async Task GetRegularTimerPage()
        {
              this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageTimer = RegularTimerPage;

            });

        }



        internal void SetControl(PanelTimer pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            TaskPages = pass;
        }


        internal void SetDispatcher(CoreDispatcher dispatcher)
        {

            this.dispatcher = dispatcher;
        }
    }
}
