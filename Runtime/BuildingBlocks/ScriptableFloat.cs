using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    [CreateAssetMenu]
    public class ScriptableFloat : ScriptableObject
    {
        enum Constraints
        {
            None,
            CannotGoBelowZero,
            ZeroToOne
        }
        
        public ScriptableVariableScope _scriptableVariableScope;
        [SerializeField] private float _initialValue;
        [SerializeField] private Constraints _constraints;

        private float trackedValue
        {
            get
            {
                if (Application.isPlaying == false)
                {
                    return _initialValue;
                }

                ScriptableFloatManager.GetState(this, out ScriptableFloatManager.State state);
                return state.value;
            }
            set
            {
                if (Application.isPlaying == false)
                {
                     _initialValue = value;
                }
                else
                {
                    ScriptableFloatManager.SetValue(this, value);
                }
            }
        }
        
        public Action<float> onValueChanged
        {
            get
            {
                if (RuntimeEditorHelper.IsQuitting)
                {
                    return null;
                }
                ScriptableFloatManager.GetState(this, out ScriptableFloatManager.State state);
                return state.onValueChanged;
            }
            set
            {
                if (RuntimeEditorHelper.IsQuitting)
                {
                    return;
                }
                
                ScriptableFloatManager.GetState(this, out ScriptableFloatManager.State state);
                state.onValueChanged = value;
                
                // if (_logEventRegistration)
                // {
                        // Debug.Log("_logEventRegistration" , this);
                // }
            }
        }


        [ShowNativeProperty]
        public float value
        {
            get
            {
                if (Application.isPlaying)
                {
                    return trackedValue;
                }
                else
                {
                    return trackedValue;
                }
            }
            set
            {
                float newValue = value;
                if (_constraints == Constraints.ZeroToOne)
                {
                    newValue = Mathf.Clamp01(newValue);
                }
                else if (_constraints == Constraints.CannotGoBelowZero)
                {
                    newValue = Mathf.Max(0, newValue);
                    
                }
                if (Math.Abs(newValue - trackedValue) > Mathf.Epsilon)
                {
                    trackedValue = newValue;
                    if (Application.isPlaying)
                    {
                        onValueChanged?.Invoke(trackedValue);
                    }
                }
            }
        }

        public float initialValue => _initialValue;
        public float intValue => value.ToInt();

        public ScriptableVariableScope scriptableVariableScope => _scriptableVariableScope;

        [Button("Increment by 0.1")]
        public void IncrementByPointOne()
        {
            value += 0.1f;
        }
        
        [Button("Increment by 1")]
        public void IncrementByOne()
        {
            value += 1f;
        }
        public void Increment(float amount)
        {
            value += amount;
        }
        
        // implicit
        public static implicit operator float(ScriptableFloat scriptableFloat)
        {
            return scriptableFloat.value;
        }

        public override string ToString()
        {
            return value + "";
        }
        
        public Coroutine IncrementValueOverTime( float increment, float duration, SoundAsset sound = null)
        {
            return AsyncHelper.StartCoroutine(IEIncrementValueOverTime(increment, duration,sound));
        }
    
        private IEnumerator IEIncrementValueOverTime(float increment, float duration, SoundAsset sound)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            float lerpPrev = 0;

            while (Time.time < endTime)
            {
                float currentTime = Time.time;
                float lerp = Mathf.Clamp01((currentTime - startTime) / duration);

                float diff = lerp - lerpPrev;
                lerpPrev = lerp;
                value += diff * increment;
                sound?.Play();
                yield return null;
            }

            // Ensure final increment in case of precision issues
            value += (1 - lerpPrev) * increment;
        }
    }
}
