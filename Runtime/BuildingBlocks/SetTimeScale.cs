using UnityEngine;


public class SetTimeScale : ActionBase
{
    [SerializeField] private float _timeScale = 1f;

    protected override void TriggerInternal()
    {
        Time.timeScale = _timeScale;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}