using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Xml.Linq;
//using Timer = System.Timers.Timer;

namespace DailyUnoThesis.Models.DobleClasses
{
    public abstract class TaskTimer:Base
    {
        private TimeSpan remainingTime;

        public DispatcherTimer Timer { get; set; }
        public bool IsPaused { get; set; } = false;
        public TimeSpan RemainingTime /*{ get; set; }*/
        {
            get => remainingTime;
            set
            {
                remainingTime = value;
                Signal();
            }
        }
        public TimeSpan SpecifiedTime { get; set; }




     
        public abstract void PauseTimer();
        public abstract void TimerEvent(object sender, object e);

        public virtual void StartTimer(int hours, int minutes, int seconds)
        {
            Timer = new DispatcherTimer(); // Таймер срабатывает каждую секунду
            Timer.Tick += TimerEvent;
            Timer.Interval = TimeSpan.FromSeconds(1);
           
            RemainingTime = new TimeSpan(hours, minutes, seconds);
            SpecifiedTime = new TimeSpan(hours, minutes, seconds);
            //Timer.Start();
            IsPaused = false;
        }

    }
}
