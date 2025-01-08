using System;
using UnityEngine;

namespace GiantSword
{
    public class SetDepth : MonoBehaviour
    {
        enum Space
        {
            Local,
            World
        }
        
        [SerializeField] private SmartFloat _depth = 0;
        [SerializeField] private Space _space = Space.Local;
        // Update is called once per frame
        void OnDrawGizmosSelected()
        {
            Apply();
        }

        public void Awake()
        {
            Apply();
        }

        private void Apply()
        {
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
    }
}
