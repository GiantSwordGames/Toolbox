using UnityEngine;

namespace JamKit
{
    public class DeactivateOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
