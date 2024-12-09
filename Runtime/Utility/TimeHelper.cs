using UnityEngine;

namespace GiantSword
{
    public static class TimeHelper
    {
        private const float PAUSE_TIME_SCALE = float.Epsilon;
        
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
       
    }
}
