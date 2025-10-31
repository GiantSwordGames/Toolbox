using System;
using System.Collections.Generic;
using JamKit;
using UnityEngine;
using UnityEngine.Serialization;

public class StaticInstanceFloatManager : AutoMonoSingleton<StaticInstanceFloatManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        gameObject.hideFlags = HideFlags.DontSave;
    }

    [Serializable]
    public class State
    {
        public float value;
    }
    Dictionary<string, State> states = new Dictionary<string, State>();
    [FormerlySerializedAs("variable")] public StaticInstanceFloat _float;

    public State GetState(string id, float defaultValue)
    {
        if (!states.ContainsKey(id))
        {
            states[id] = new State();
            states[id].value = defaultValue;
        }
        return states[id];
    }
}

[Serializable]
public class StaticInstanceFloat
{
    [SerializeField] private StaticInstanceFloatManager.State _state = new StaticInstanceFloatManager.State();
    [SerializeField] private string _id = "";

    public float value
    {
        get => _state.value;
        set { _state.value = value; }
    }

    public string id => _id;


    public StaticInstanceFloat(Component component,string variableName, float _defaultValue)
    {
        _id = component.GetFullHierachyPath() + "_" + variableName;
        _state = StaticInstanceFloatManager.Instance.GetState(_id, _defaultValue);
        
    }

}


[Serializable]
public class StaticInstanceInt : StaticInstanceFloat
{
    public new int value
    {
        get => base.value.ToInt();
        set => base.value = value;
    }

    public StaticInstanceInt(Component component,string variableName, int _defaultValue) : base(component, variableName, _defaultValue)
    {
    }
}
public class StaticInstanceBool : StaticInstanceFloat
{
    public StaticInstanceBool(Component component, string variableName, bool defaultValue) : base(component, variableName, defaultValue ? 1 : 0)
    {
    }

    public new bool value
    {
        get => base.value != 0;
        set =>  base.value = value ? 1 : 0;
    }
}