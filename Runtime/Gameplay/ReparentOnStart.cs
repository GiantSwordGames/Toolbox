using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class ReparentOnStart : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [ShowNonSerializedField]  private Transform _formerParent;
        private void Awake()
        {
        }

        private void Start()
        {
            _formerParent = transform.parent;
            if (_formerParent)
            {
                _formerParent.gameObject.AddComponent<ReparentReference>().formerChild = this;
            }
            transform.SetParent(_parent);
        }
    }
}
