using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public class PunchInstance
    {
        private Transform _transform;
        private PunchScaleAsset _asset;
        private Coroutine _routine;
        Vector3 _offset = Vector3.zero;

        public PunchInstance(Transform transform, PunchScaleAsset asset )
        {
            _transform = transform;
            _asset = asset;
            _routine = AsyncHelper.StartCoroutine(Run( asset.amplitudeVector, asset.oscilations, asset.duration));
            
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
        

        private  IEnumerator Run( Vector3 amplitude, int oscillations, float duration)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float lerp = time / duration;
                float decay = 1 - lerp;
                float t = Mathf.Sin(lerp * Mathf.PI * oscillations);

                _transform.localScale -= _offset;
                _offset = amplitude * t*decay;
                _transform.localScale += _offset;
                yield return null;
            }
            
            _transform.localScale -= _offset;
            _offset = Vector3.zero;
        }

    }
}