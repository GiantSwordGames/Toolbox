using UnityEngine;

namespace GiantSword
{
    public class PixelSnappingElement: MonoBehaviour
    {
        [SerializeField] private bool _dontSnap;

        public bool dontSnap => _dontSnap;
    }
}