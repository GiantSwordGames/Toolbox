using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class RotateGameObject : MonoBehaviour
    {
        [Button]
        void Rotate90()
        {
            transform.localRotation *= Quaternion.Euler(0,0,90);
        }
    }
}
