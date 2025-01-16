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
        private Transform _transform;
        private PunchAsset _asset;
        private Coroutine _routine;
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

        public void Kill()
        {
            if (_routine != null)
            {
                AsyncHelper.StopRoutine(_routine);
                _routine = null;
            }
            _transform.localScale -= _offset;
            _offset = Vector3.zero;
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
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float lerp = time / duration;
                float decay = 1 - lerp;
                float t = Mathf.Sin(lerp * Mathf.PI * oscillations);

                function(-_offset);
                _offset = amplitude * t*decay;
                function(_offset);

                yield return null;
            }
            
            function(-_offset);

            _offset = Vector3.zero;
        }
    }
}