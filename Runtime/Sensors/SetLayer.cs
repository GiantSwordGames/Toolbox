using UnityEngine;

namespace JamKit
{
    public class SetLayer:MonoBehaviour
    {
        [SerializeField] private int _layer;
        [SerializeField] private GameObject _target;
        [SerializeField] private bool _includeChildren = true;

        public void Trigger()
        {
            if (_includeChildren)
            {
                foreach (Transform t in _target.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = _layer;
                }
            }
            else
            {
                _target.layer = _layer;
            }
        }
    }
}