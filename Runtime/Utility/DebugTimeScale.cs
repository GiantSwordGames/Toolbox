using JamKit;
using UnityEngine;

namespace RichardPieterse
{
    public class DebugTimeScale : MonoBehaviour
    {
        const float SUPER_SLOW_TIME_SCALE = 0.05f;
        const float SLOW_TIME_SCALE = 0.1f;
        
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightShift))
            {
                if (TimeHelper.timeScale > SLOW_TIME_SCALE)
                {
                    TimeHelper.SetDebugTimeScale(SLOW_TIME_SCALE);
                }
                else  if (TimeHelper.timeScale > SUPER_SLOW_TIME_SCALE)
                {
                    TimeHelper.SetDebugTimeScale(SUPER_SLOW_TIME_SCALE);
                }
                else
                {
                    TimeHelper.SetDebugTimeScale(1);
                }
            }

        }
    }
}
