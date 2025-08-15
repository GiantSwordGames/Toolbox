using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class Toggle : MonoBehaviour
    {
        [Button]
        public void Trigger()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}