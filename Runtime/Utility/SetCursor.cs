using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class SetCursor : MonoBehaviour
    {
        [SerializeField] private CursorLockMode _lockstate;
        [SerializeField] private bool _visible;
        [SerializeField] private Texture2D _customCursor;

        private void OnEnable()
        {
            Cursor.lockState = _lockstate;
            Cursor.visible = _visible;

            if (_customCursor != null)
            {
                Cursor.SetCursor(_customCursor, new Vector2(_customCursor.width/2, _customCursor.height/2), CursorMode.Auto);
            }
        }
    }
}
