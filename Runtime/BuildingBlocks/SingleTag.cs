using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class SingleTag : MonoBehaviour
    {
        [FormerlySerializedAs("_tag")] [SerializeField] private TagAsset _tag;
        public new TagAsset tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
