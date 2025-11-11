using NaughtyAttributes;
using UnityEngine;

enum TriggerOn
{
    Manual,
    OnEnable,
    OnDisable,
    Start,
    Awake,
}
public abstract class ActionBase : MonoBehaviour
{
    [SerializeField] private TriggerOn _triggerOn;
    protected virtual void Awake()
    {
        if (_triggerOn == TriggerOn.Awake)
        {
            TriggerInternal();
        }
    }
    protected virtual  void OnEnable()
    {
        if (_triggerOn == TriggerOn.OnEnable)
        {
            TriggerInternal();
        }
    }
    protected virtual  void Start()
    {
        if (_triggerOn == TriggerOn.Start)
        {
            TriggerInternal();
        }
    }
    protected virtual  void OnDisable()
    {
        if (_triggerOn == TriggerOn.OnDisable)
        {
            TriggerInternal();
        }
    }
    
    
    protected abstract void TriggerInternal();
   
    [Button]
    private void Trigger()
    {
        if (enabled)
        {
            TriggerInternal();
        }
    }
}