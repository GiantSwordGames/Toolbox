using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    [ExecuteInEditMode]
    public class SetDepth : MonoBehaviour
    {
        enum Space
        {
            Local,
            World
        }
        
        [SerializeField] private SmartFloat _depth = 0;
        [SerializeField] private Space _space = Space.Local;
        [SerializeField] private bool _updateAtRuntime;
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
            if (_space == Space.Local)
            {
                if (transform.localPosition.z != _depth)
                {
                    transform.SetLocalScaleZ(_depth);
                    RuntimeEditorHelper.SetDirty(transform);
                }
            }
            else
            {
                if (transform.position.z != _depth)
                {
                    transform.SetZ(_depth);
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
