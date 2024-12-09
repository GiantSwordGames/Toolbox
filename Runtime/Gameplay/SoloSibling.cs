using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SoloSibling : MonoBehaviour
    {
        [SerializeField] private Transform _sibling;

        [Button]
        public void Trigger()
        {
            if (_sibling)
            {
                Solo solo = _sibling.GetComponent<Solo>();
                if (solo)
                {
                    solo.Trigger();
                }
                else
                {
                    _sibling.Solo();
                }
            }
        }
    }
}
