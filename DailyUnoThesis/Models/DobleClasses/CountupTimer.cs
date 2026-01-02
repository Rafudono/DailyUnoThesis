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


        public override void TimerEvent(object sender, object e)
        {
                RemainingTime += TimeSpan.FromSeconds(1); // Увеличиваем времени на 1 секунду                                             
        }
    }
}
