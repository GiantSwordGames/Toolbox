using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class SoloSibling : MonoBehaviour
    {
        [FormerlySerializedAs("_sibling")] [SerializeField] private Transform _target;

        [Button]
        public void Trigger()
        {
            if (enabled == false)
            {
                return;
            }

            if (_target == null)
            {
                _target = transform;
            }

            if (_target)
            {
                Solo solo = _target.GetComponent<Solo>();
                if (solo)
                {
                    solo.Trigger();
                }
                else
                {
                    _target.Solo();
                }
            }
        }
    }
}
