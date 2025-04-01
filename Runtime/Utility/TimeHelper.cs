using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public enum TimeScale
    {
        Scaled,
        Unscaled
    }
    
    public static class TimeHelper
    {
        
        private const float PAUSE_TIME_SCALE = float.Epsilon;
        public const float SECONDS_IN_A_DAY = 86400;
        
        public static float timeScale
        {
            get => Time.timeScale;
            set => Time.timeScale = value;
        }

        public static void PausePhysics()
        {
            timeScale = PAUSE_TIME_SCALE;
        }
        
        public static void UnpausePhysics()
        {
            timeScale = 1;
        }
        
        public static void SetDebugTimeScale(float _timeScale)
        {
            timeScale = _timeScale;
        }

        public static float GetHours(float seconds)
        {
            return seconds / 3600;
        }
        
        public static float HoursToSeconds(float hours)
        {
            return hours * 3600;
        }
        
        public static string GetTimeOfDay(float seconds)
        {
            float hours = GetHours(seconds);
            int hoursInt = (int)hours;
            int minutes = (int)((hours - hoursInt) * 60);
            
            bool isPm = hoursInt >= 12;
            
            if (hoursInt > 12)
            {
                hoursInt -= 12;
            }
            else if (hoursInt == 0)
            {
                hoursInt = 0;
            }

            return $"{hoursInt}:{minutes:D2} {(isPm ? "PM" : "AM")}";
        }
        
        public static object SmartWaitForSeconds(float seconds, TimeScale timeScale)
        {
            if (timeScale == TimeScale.Unscaled)
            {
                return new WaitForSecondsRealtime(seconds);
            }
            else
            {
                return new WaitForSeconds(seconds);
            }
        }

        public static float GetDeltaTime(TimeScale scale)
        {
            return scale == TimeScale.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;
        }
    }
}
