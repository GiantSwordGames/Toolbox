using System;
using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public class PunchInstance
    {
        public enum Type
        {
            Scale,
            Position
        }

        public enum State
        {
            NotStarted,
            Running,
            Complete,
            Killed
        }
        
        public Vector3 instanceScale = Vector3.one;

        private Transform _transform;
        private PunchAsset _asset;
        private Coroutine _routine;
        private State _state;
        
        public event Action onKill;
        public event Action onComplete;
        public event Action onUpdate;
        
        Vector3 _offset = Vector3.zero;

        public PunchInstance(Transform transform, PunchAsset asset, Type type)
        {
            _transform = transform;
            _asset = asset;

            Action<Vector3> function = ApplyToScale;
            if (type == Type.Position)
            {
                function = ApplyToPosition;
            }
            _routine = AsyncHelper.StartCoroutine(Apply( asset.amplitudeVector, asset.oscilations, asset.duration, function));
        }

        public State state => _state;

        public Coroutine routine => _routine;

        public void Kill()
        {
            if (_state != State.Running)
            {
                return;
            }
            
            if (_routine != null)
            {
                AsyncHelper.StopRoutine(_routine);
                _routine = null;
            }
            _transform.localScale -= _offset;
            _offset = Vector3.zero;

            _state = State.Killed;
            onKill?.Invoke();
        }
        
        private void ApplyToScale(Vector3 value)
        {
            _transform.localScale += value;
        }
        
        private void ApplyToPosition(Vector3 value)
        {
            _transform.localPosition += value;
        }
       
        private  IEnumerator Apply(  Vector3 amplitude, int oscillations, float duration, Action<Vector3> function)
        {
            _state = State.Running;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float lerp = time / duration;
                float decay = 1 - lerp;
                float t = Mathf.Sin(lerp * Mathf.PI * oscillations);

                function(-_offset);
                _offset = amplitude * t*decay;
                _offset.Scale(instanceScale);
                function(_offset);
                onUpdate?.Invoke();

                yield return null;
            }
            
            function(-_offset);
            _offset = Vector3.zero;
            onUpdate?.Invoke();

            _state = State.Complete;
            onComplete?.Invoke();
            
        }
    }
}