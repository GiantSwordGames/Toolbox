using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class SetCursorStateOnEnable : MonoBehaviour
    {
        [SerializeField] private CursorLockMode _lockstate;
        [FormerlySerializedAs("_visibility")] [SerializeField] private bool _visible;

        private void OnEnable()
        {
            Cursor.lockState = _lockstate;
            Cursor.visible = _visible;
        }
    }
}
