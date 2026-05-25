using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class CountupTimer : TaskTimer
    {
        public DateTime StartPlayTimer { get; set; }
        public DateTime EndTimer { get; set; }
        public override void PauseTimer()
        {
            if (!IsPaused && Timer.IsEnabled)
            {

                Timer.Stop(); // Паузируем таймер
                IsPaused = true;
                EndTimer = DateTime.Now;    
            }
            else
            {
                Timer.Start(); // Продолжаем таймер
                IsPaused = false;
            }
            Restart = IsPaused;
        }


        public override void TimerEvent(object sender, object e)
        {
                RemainingTime += TimeSpan.FromSeconds(1); // Увеличиваем времени на 1 секунду                                             
        }
    }
}
