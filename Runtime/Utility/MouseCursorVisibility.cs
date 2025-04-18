using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class MouseCursorVisibility : MonoBehaviour
    {
        [SerializeField] private CursorLockMode _lockstate;
        [SerializeField] private bool _visible;

        private void OnEnable()
        {
            Cursor.lockState = _lockstate;
            Cursor.visible = _visible;
        }
    }
}
