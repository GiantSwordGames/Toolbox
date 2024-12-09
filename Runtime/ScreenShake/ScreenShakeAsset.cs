using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    [Serializable]
    public class Oscillator
    {
        [SerializeField] public Vector3 componentAmplitude = new Vector3(1, 1, 0);
        [SerializeField] public Vector3 componentFrequency= new Vector3(1, 1, 0);
        [SerializeField] public float frequency = 1;
        [SerializeField] public float amplitude = 1;

        public Oscillator(float frequency, float amplitude, Vector3 componentFrequency, Vector3 componentAmplitude)
        {
            this.frequency = frequency;
            this.amplitude = amplitude;
            this.componentFrequency = componentFrequency;
            this.componentAmplitude = componentAmplitude;
        }

        public Oscillator()
        {
        }

        public Vector3 Evaluate(float time)
        {
            Vector3 result = Vector3.zero;
            result.x = Mathf.Sin(time * componentFrequency.x * frequency)*componentAmplitude.x;
            result.y = Mathf.Sin(time * componentFrequency.y * frequency)*componentAmplitude.y;
            result.z = Mathf.Sin(time * componentFrequency.z * frequency)*componentAmplitude.z;
            result *= amplitude;
            return result;
        }
    }

    public class ScreenShakeAsset : ScriptableObject
    {
        public event Action listenForButtonTest;
        [SerializeField] private AnimationCurve _decay = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private float _duration = 1;
        [SerializeField] private float _amplitude = 1;

        [SerializeField] private Oscillator _positionOscillator = new Oscillator(16, 1, new Vector3(1, 2, 0), new Vector3(1, 1, 0));
        [SerializeField] private Oscillator _rotationOscillator = new Oscillator(1, 0, new Vector3(0, 0, 1), new Vector3(0, 0, 1));
        private Coroutine _repeatRoutine;

        public float duration => _duration;

        public Vector3 EvaluatePosition(float time)
        {
            return  EvaluateOscillator(_positionOscillator, time);
        }

        private Vector3 EvaluateOscillator(Oscillator oscillator, float time)
        {
            Vector3 result = Vector3.zero;
            result += oscillator.Evaluate(time);
            result *= _amplitude;
            result *= _decay.Evaluate(Mathf.Clamp01(time / _duration));
            return result;
        }

        public Vector3 EvaluateRotation(float time)
        {
            return  EvaluateOscillator(_rotationOscillator, time);
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Trigger()
        {
            listenForButtonTest?.Invoke();
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void TriggerRepeating()
        {
            Stop();
           _repeatRoutine = AsyncHelper.StartCoroutine(IERepeatTrigger());
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Stop()
        {
            if (_repeatRoutine != null)
            {
                AsyncHelper.StopRoutine(_repeatRoutine);
                _repeatRoutine = null;
            }
            
        }

        private IEnumerator IERepeatTrigger()
        {
            while (true)
            {
                Trigger();
                yield return new WaitForSeconds(_duration*2);
            }
        }
    }
}
