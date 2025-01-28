using UnityEngine;

namespace GiantSword
{
    public class DeparentOnEnable : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.SetParent(null);
        }
    }
}