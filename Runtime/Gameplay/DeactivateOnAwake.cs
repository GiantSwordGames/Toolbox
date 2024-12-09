using UnityEngine;

namespace GiantSword
{
    public class DeactivateOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
