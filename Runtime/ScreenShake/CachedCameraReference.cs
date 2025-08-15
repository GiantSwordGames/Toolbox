using UnityEngine;

namespace GiantSword
{
    public class CameraReference
    {
        private static Camera _mainCamera;
        public static Camera mainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }
                return _mainCamera;
            }
        }

    }
}