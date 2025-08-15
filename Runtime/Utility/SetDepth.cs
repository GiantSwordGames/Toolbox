using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    [ExecuteInEditMode]
    public class SetDepth : MonoBehaviour
    {
        enum Space
        {
            Local,
            World
        }
        
        [SerializeField] private SmartFloat _depth = new SmartFloat( 0);
        [SerializeField] private Space _space = Space.Local;
        [SerializeField] private bool _updateAtRuntime;
        [FormerlySerializedAs("_localYMultiplier")] [FormerlySerializedAs("_multiplier")] [SerializeField] private float _yMultiplier = 0;
        
        
        // Update is called once per frame
        void Update()
        {
            if (Application.isPlaying == false || _updateAtRuntime)
            {
                Apply();
            }
        }

        public void Awake()
        {
            Apply();
        }

        [Button]
        public void Apply()
        {
            if (enabled == false)
            {
                
                return;
            }

            float depth = _depth+ transform.position.y*_yMultiplier;
            if (_space == Space.Local)
            {
                if (transform.localPosition.z != depth)
                {
                    transform.SetLocalZ(depth);
                    RuntimeEditorHelper.SetDirty(transform);
                }
            }
            else
            {
                if (transform.position.z != depth)
                {
                    transform.SetZ(depth);
                    RuntimeEditorHelper.SetDirty(transform);
                }
            }
            
        }

        public void ApplyTemporaryValue(float value)
        {
            transform.SetZ(value);
        }
    }
}
