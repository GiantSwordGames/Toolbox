using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class InstantiatePrefab : MonoBehaviour
    {
        [Button]
        private void Instantiate()
        {
            Instantiate(gameObject);
        }
    }
}
