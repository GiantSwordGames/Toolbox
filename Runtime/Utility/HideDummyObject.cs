using UnityEngine;

namespace JamKit
{
    public class HideDummyObject : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
