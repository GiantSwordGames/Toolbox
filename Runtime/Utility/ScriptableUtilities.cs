using NaughtyAttributes;

namespace GiantSword
{
    public class ScriptableUtilities : ScriptableSingleton<ScriptableUtilities>
    {
        [Button]
        public void FreezeTime()
        {
            TimeHelper.PausePhysics();
        }

        [Button]
        public  void UnfreezeTime()
        {
            TimeHelper.UnpausePhysics();
        }
        
        public  void SlowTime(float timeScale)
        {
            TimeHelper.timeScale = timeScale;
        }
    }
}