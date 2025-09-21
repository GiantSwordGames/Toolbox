using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public abstract class DoPunch : MonoBehaviour
    {
        [Button]
        public virtual void Trigger()
        {
            
        }
    }
}