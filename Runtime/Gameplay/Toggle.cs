using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
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