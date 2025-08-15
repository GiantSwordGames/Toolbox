
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public abstract class CustomEditorBase<T> : UnityEditor.Editor where T : Object
    {
        private T _targetObject;

        public T targetObject
        {
            get
            {
                if (_targetObject == null)
                {
                    _targetObject = target as T;
                }
                return _targetObject;
            }
        }

        protected IEnumerable<T> targetObjects
        {
            get
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] is T obj)
                        yield return obj;
                }
            }
        }

        protected virtual void OnDisable()
        {
            _targetObject = null;
        }
    }
}