using System;
using System.Collections;
using GiantSword;
using UnityEngine;

[System.Serializable]
public class SmartFloat
{
    public enum Mode
    {
        Constant,
        Variable,
        MonoFloat,
        ConfigurationFloat,
        FloatRange,
        FloatVariance,
    }

    public Mode _mode; // Use an enum instead of a bool
    public float _constantValue;
    public ConfigurationFloat _configurationFloat; // Assuming this has a "value" field or property
    public ScriptableFloat _variable;
    public MonoFloat _monoFloat;
    public FloatRange _floatRange;
    public FloatVariance _floatVariance;

    public Action<float> onConstantValueChanged;
    public Action<float> onValueChanged
    {
        get
        {
            switch (_mode)
            {
                case Mode.Variable:
                    return _variable.onValueChanged;
                case Mode.MonoFloat:
                    return _monoFloat.onValueChangedAction;
                case Mode.ConfigurationFloat:
                    return _configurationFloat.onValueChanged;
                case Mode.Constant:
                    return onConstantValueChanged;
                default:
                    return null;
            }
        }
        set
        {
            switch (_mode)
            {
                case Mode.Variable:
                    _variable.onValueChanged = value;
                    break;
                case Mode.MonoFloat:
                    _monoFloat.onValueChangedAction = value;
                    break;
                case Mode.ConfigurationFloat:
                    _configurationFloat.onValueChanged = value;
                    break;
                case Mode.Constant:
                    onConstantValueChanged = value;
                    break;
            }
        }
    }

    public SmartFloat(Mode mode)
    {
        _mode = mode;
    }

    public SmartFloat(MonoFloat health)
    {
        _mode = Mode.MonoFloat;
        _monoFloat = health;
    }

    private SmartFloat()
    {
    }

    public float value
    {
        get
        {
            switch (_mode)
            {
                case Mode.Constant:
                    return _constantValue;
                case Mode.Variable:
                    return _variable != null ? _variable.value : 0f;
                case Mode.MonoFloat:
                    return _monoFloat != null ? _monoFloat.value : 0f;
                case Mode.ConfigurationFloat: // New case for ConfigurationFloat
                    return _configurationFloat != null ? _configurationFloat.value : 0f;
                case Mode.FloatRange :
                    return  _floatRange.value;
                
                default:
                    return 0f;
            }
        }
        set
        {
            float previous = this.value;
            switch (_mode)
            {
                case Mode.Constant:
                    _constantValue = value;
                    if(!Mathf.Approximately(previous, value))
                        onConstantValueChanged?.Invoke(value);
                    break;
                case Mode.Variable:
                    if (_variable != null)
                    {
                        _variable.value = value;
                    }
                    break;
                case Mode.MonoFloat:
                    if (_monoFloat != null)
                    {
                        _monoFloat.value = value;
                    }
                    break;
                case Mode.ConfigurationFloat: // New setter case for ConfigurationFloat
                    if (_configurationFloat != null)
                    {
                        _configurationFloat.value = value; // Assuming _configurationFloat has a value property
                    }
                    break;
            }
            
        }

    }

    public override string ToString()
    {
        return value.ToString();
    }

    public string name
    {
        get
        {
            switch (_mode)
            {
                case Mode.Constant:
                    return "SmartFloat Constant";
                case Mode.Variable:
                    return _variable != null ? _variable.name : "";
                case Mode.MonoFloat:
                    return _monoFloat != null ? _monoFloat.name : "";
                case Mode.ConfigurationFloat:
                    return _configurationFloat != null ? _configurationFloat.name : "";
                default:
                    return "";
            }
            
        }
    }

    // implicit operator to convert float to SmartFloat
    public static implicit operator SmartFloat(float value)
    {
        return new SmartFloat
        {
            _mode = Mode.Constant,
            _constantValue = value
        };
    }
    
    // implicit operator to convert float to SmartFloat
    public static implicit operator SmartFloat(ScriptableFloat scriptableFloat)
    {
        return new SmartFloat
        {
            _mode = Mode.Variable,
            _variable = scriptableFloat
        };
    }

    public float Round(int digits = 0)
    {
        return (float) Math.Round(value, digits);
    }

    // implicit operator to convert SmartFloat to float
    public static implicit operator float(SmartFloat smartFloat)
    {
        return smartFloat.value;
    }

    public Coroutine IncrementValueOverTime( float increment, float duration)
    {
        return AsyncHelper.StartCoroutine(IEIncrementValueOverTime(increment, duration));
    }
    private IEnumerator IEIncrementValueOverTime(float increment, float duration)
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

            yield return null;
        }

        // Ensure final increment in case of precision issues
        value += (1 - lerpPrev) * increment;
    }
}
