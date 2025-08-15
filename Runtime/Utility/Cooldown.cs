using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    [SerializeField]  private float _duration;

    private float _timeStamp;
    private bool _initialized;

    public float timeStamp => _timeStamp;

    public float duration
    {
        get => _duration;
        set => _duration = value;
    }

    public Cooldown(float duration)
    {
        _duration = duration;
        _timeStamp =0;
        _initialized = false;
    }
    
    public float GetRemainingTime()
    {
        CheckInitialization();
        return Mathf.Max(0, _timeStamp + _duration - Time.time);
    }
    
    public Cooldown(float duration, bool isReady)
    {
        _duration = duration;
        _timeStamp =0;
        _initialized = false;

        if (isReady)
        {
            _initialized = true;
            _timeStamp = -duration;
        }
    }

    public void Reset()
    {
        _timeStamp = Time.time;
        _initialized = true;
    }

    public bool HasElapsed()
    {
        CheckInitialization();
        return Time.time > _timeStamp + _duration;
    }

    private void CheckInitialization()
    {
        if (_initialized == false)
        {
            Reset();
        }
    }

    public float GetFillAmount()
    {
        if (_duration <= 0)
        {
            return 1f;
        }
        
        CheckInitialization();

        float elapsed = Time.time - _timeStamp;
        return Mathf.Clamp01(elapsed / _duration);
    }

    public void SetToAlmostComplete(int timeRemaining)
    {
        if (timeRemaining < 0) 
            timeRemaining = 0;
        _timeStamp = Time.time + _duration - timeRemaining;
    }
    
    public static implicit operator bool(Cooldown cooldown)
    {
        return cooldown.HasElapsed();
    }

    public void Randomize()
    {
        _timeStamp = Time.time + UnityEngine.Random.Range(0, _duration);
        _initialized = true;
    }

    public void Complete()
    {
        _timeStamp = Time.time - _duration;
        _initialized = true;
    }
}