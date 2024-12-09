namespace GiantSword
{
   

    //
    // public abstract class ScriptableVariableBase : ScriptableObject
    // {
    //    
    //     [ShowNonSerializedField]  protected bool _initialized = false;
    //
    //     [FormerlySerializedAs("_scope")] [SerializeField] private ScriptableVariableScope _scriptableVariableScope = ScriptableVariableScope.PlayerDied;
    //     public ScriptableVariableScope scriptableVariableScope => _scriptableVariableScope;
    //
    //     
    //     [Button]
    //     public void ClearDomain()
    //     {
    //         _initialized = false;
    //     }
    //
    //     public abstract void Reset();
    // }
    //
    // public abstract class ScriptableVariable<T> : ScriptableVariableBase where T : struct
    // {
    //     [SerializeField] private T _initialValue;
    //
    //     private T _currentValue
    //     {
    //         get
    //         {
    //
    //             ScriptableBoolManager.GetValue(this, out T value);
    //         }
    //     }
    //     public event Action<T> onChanged;
    //     
    //     
    //     public T value
    //     {
    //         get
    //         {
    //             if (Application.isPlaying)
    //             {
    //                 Initialize();
    //                 return _currentValue;
    //             }
    //             else
    //             {
    //                 return _initialValue;
    //             }
    //         }
    //         set
    //         {
    //             Initialize();
    //             if (value.Equals( _currentValue) == false)
    //             {
    //                 _currentValue = value;
    //                 onChanged?.Invoke(_currentValue);
    //             }
    //         }
    //     }
    //
    //     public T initialValue => _initialValue;
    //
    //   
    //
    //
    //     
    //
    //     
    //     public override void Reset()
    //     {
    //         onChanged = null;
    //         _currentValue = _initialValue; 
    //     }
    // }
    //
   
}