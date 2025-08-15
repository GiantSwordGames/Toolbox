using NaughtyAttributes;
using UnityEngine;

namespace JamKit
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
