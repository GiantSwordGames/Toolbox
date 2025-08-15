using UnityEngine;

namespace JamKit
{
    public class DeparentOnEnable : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.SetParent(null);
        }
    }
}