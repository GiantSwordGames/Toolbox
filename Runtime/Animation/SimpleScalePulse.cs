using UnityEngine;

public class SimpleScalePulse : MonoBehaviour
{
    private Vector3 _startScale;
    [SerializeField] private float _pulseAmplitude = 0.1f;
    [SerializeField] private float _pulseTimeOffset = 0;
    [SerializeField] private float _pulseFrequency = 4;
    [SerializeField] private bool _useUnscaledTime = false;
    void Start()
    {
        _startScale = transform.localScale;
    }
    void Update()
    {
        transform.localScale = _startScale + Vector3.one * Mathf.Sin(((_useUnscaledTime ? Time.unscaledTime : Time.time) + _pulseTimeOffset) * _pulseFrequency) * _pulseAmplitude;
    }
}
