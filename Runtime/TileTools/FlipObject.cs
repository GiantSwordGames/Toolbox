using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class FlipObject : MonoBehaviour
    {

        [Button]
        public void FlipX()
        {
            transform.localScale = transform.localScale.WithX(-transform.localScale.x);
        }
        
        [Button]
        public void FlipY()
        {
            transform.localScale = transform.localScale.WithY(-transform.localScale.y);
        }
    }
}
