using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    [Serializable]
    public struct TargetTransform
    {
        public enum Mode
        {
            Self, 
            Other, 
        }

        [SerializeField] private Mode _mode;
        [FormerlySerializedAs("_target")] 
        [SerializeField] private Transform _otherTransform;
        [SerializeField] private Transform _self;
        public bool isAvaliable => target != null;

        public void Initialize(Component self)
        {
            _self = self.transform;
        }

        public Transform target
        {
            get
            {
                switch (_mode)
                {
                    case Mode.Self:
                        return _self;
                    case Mode.Other:
                        return _otherTransform;
                  
                        return null;
                }

                return null;
            }
            set
            {
                _otherTransform = value;
                _mode = Mode.Other;
            }
        }

        public Vector3 localPosition
        {
            get => target.localPosition;
            set => target.localPosition = value;
        }
        
        public Vector3 position
        {
            get => target.position;
            set => target.position = value;
        }
        
        public Quaternion localRotation
        {
            get => target.localRotation;
            set => target.localRotation = value;
        }
        
        public Vector3 localScale
        {
            get => target.localScale;
            set => target.localScale = value;
        }
        
        public static implicit operator Transform(TargetTransform targetTransform)
        {
            return targetTransform.target;
        }
        
        public static implicit operator bool(TargetTransform targetTransform)
        {
            return targetTransform.target != null;
        }

        
    }
}