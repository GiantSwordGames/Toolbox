using UnityEngine;

namespace GiantSword
{
    public class DebugBreakAction : MonoBehaviour
    {
        public void Trigger()
        {
            Debug.Break();
        }
    }
}
