using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    [Serializable]
    public class SmartTween
    {
        public enum TweenType
        {
            Linear,
            PingPong,
            Sin,
            AnimationCurve
        }

        [SerializeField] private TweenType _type = TweenType.Sin;
        [SerializeField] private float _frequency = 1.0f; // Frequency of the sine wave
        [SerializeField] private float _amplitude = 0.5f; // Amplitude of the sine wave
        [SerializeField] private float _frequencyOffset = 0f;
        [SerializeField] private float _amplitudeOffset = 0f;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _clamp = false;
        
        [ShowIf("_type", TweenType.AnimationCurve)]
        [SerializeField] private AnimationCurve _animationCurve;

        
        public float Evaluate(float time)
        {
            if (_delay > 0)
            {
                time -= _delay;
                time = Mathf.Max(0, time);
            }
            
            if (_clamp)
            {
                time = Mathf.Clamp01(time);
            }
            
            time *= _frequency;
            
            time += _frequencyOffset;

            float value = 0;
            switch (_type)
            {
                
                case TweenType.Linear:
                    value = time;
                    break;
                case TweenType.PingPong:
                    value = Mathf.PingPong(time, 1);
                    break;

                case TweenType.Sin:
                    value = Mathf.Sin( time*Mathf.PI*2);
                    break;

                case TweenType.AnimationCurve:
                    value = _animationCurve.Evaluate(time);
                    break;

            }

            return value * _amplitude + _amplitudeOffset;
        }

    }
}