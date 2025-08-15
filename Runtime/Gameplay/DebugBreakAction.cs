using UnityEngine;

namespace JamKit
{
    public class DebugBreakAction : MonoBehaviour
    {
        public void Trigger()
        {
            Debug.Break();
        }
    }
}
