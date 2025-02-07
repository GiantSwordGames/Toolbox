using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class PixelSnappingElement: MonoBehaviour
    {
        [SerializeField] private bool _dontSnap;
        [SerializeField] private bool _snapScale;
        [ShowNonSerializedField] private Vector2 _pixelSize;
        [ShowNonSerializedField] private Vector3 _pixelPosition;
        private const float PIXELS_PER_UNIT = 16;

        public bool dontSnap => _dontSnap;

        public bool snapScale => _snapScale;

        public Vector2 pixelSize
        {
            get => _pixelSize;
            set => _pixelSize = value;
        }

        public Vector3 pixelPosition
        {
            get => _pixelPosition;
            set => _pixelPosition = value;
        }

        private void OnValidate()
        {
            if (pixelSize != Vector2.zero && snapScale)
            {
                // RuntimeEditorHelper.RecordObjectUndo(transform);
                // transform.localScale = pixelSize/PIXELS_PER_UNIT;
            }
            
            if (pixelPosition != Vector3.zero)
            {
                // RuntimeEditorHelper.RecordObjectUndo(transform);
                // transform.position = pixelPosition/PIXELS_PER_UNIT;
            }
        }
    }
}