using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GiantSword
{
    [System.Serializable]
    public struct FloatVariance
    {
        [SerializeField] private float _value;
        [SerializeField] private float _variance;
        
        public FloatVariance(float value, float variance)
        {
            _value = value;
            _variance = variance;
        }
        public FloatVariance(FloatRange floatRange)
        {
            _value = (floatRange.min + floatRange.max)/2;
            _variance = (float)Math.Round( ((floatRange.max - floatRange.min)/2), 5);
        }
        
        public float GetRandom()
        {
            return Random.Range(_value - _variance, _value + _variance);
        }   
        
    }
}