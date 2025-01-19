using UnityEngine;

namespace GiantSword
{
    public enum Range
    {
        Constant = 0,
        MinMax,
        Variance,
    }

    [System.Serializable]
    public struct FloatRange
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;
        private float _lastRandom;
        private bool _initialzed;

        public FloatRange(float min, float max)
        {
            _min = min;
            _max = max;
            _lastRandom = 0;
            _initialzed = false;
        }

        public float min
        {
            get => _min;
            set => _min = value;
        }

        public float max
        {
            get => _max;
            set => _max = value;
        }

        public float center => (_min + _max) * 0.5f;
        public float size => _max - _min;

        public float lastRandom => _lastRandom;

        public float GetRandom()
        {
            _lastRandom = Random.Range(_min, _max);
            return _lastRandom;
        }

        public float value
        {
            get
            {
                if (_initialzed == false)
                {
                    _initialzed = true;
                    GetRandom();
                }

                return _lastRandom;
            }
        }

        public bool IsInRange(float x)
        {
            return x >= _min && x <= _max;
        }

        public float Clamp(float x)
        {
            return Mathf.Clamp(x, _min, _max);
        }

        public static FloatRange MinMax(float min, float max)
        {
            return new FloatRange(min, max);
        }

        public float GetNormalized(float value)
        {
            return Mathf.InverseLerp(_min, _max, value);
        }
        
        public float EvaluateUnclamped(float progressBarValue)
        {
            return Mathf.Lerp(_min, _max, progressBarValue);
        }
    }
}