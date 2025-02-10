using UnityEngine;

namespace GiantSword
{
    public class SingleTag : MonoBehaviour
    {
        [SerializeField] private TagAsset _tag;
        public TagAsset tags => _tag;
    }
}
