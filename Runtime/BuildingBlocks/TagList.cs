using System.Collections.Generic;
using UnityEngine;

namespace JamKit
{
    public class TagList : MonoBehaviour
    {
        [SerializeField] private List<TagAsset> _tags;
        public List<TagAsset> tags => _tags;
    }
}