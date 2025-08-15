using System;
using JamKit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPressed;
    [SerializeField]  [FormerlySerializedAs("inputAction")] private InputActionReference _inputAction;
    // [SerializeField] private InputKeyAsset _asset;
    [SerializeField] KeyCode _keyCode = KeyCode.None;
    [SerializeField] bool _skipFirstFrame = false;
    [SerializeField] float _acceptDelayAfterAppearing = -1f;
    bool _skippedFirstFrame = false;
    private float _appearTime;

    void OnEnable()
    {
        if (_inputAction != null)
        {
            _inputAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (_inputAction != null)
        {
            _inputAction.action.Disable();
        }
    }

    private void Start()
    {
        _appearTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (_acceptDelayAfterAppearing > 0)
        {
            if(Time.realtimeSinceStartup - _appearTime < _acceptDelayAfterAppearing)
            {
                return;
            }
        }
        
        if (_skipFirstFrame && _skippedFirstFrame ==false)
        {
            _skippedFirstFrame = true;
                return;
        }
        if (_inputAction != null && _inputAction.action.triggered)
        {
            Trigger();
            return;
        }

        if (Input.GetKeyDown(_keyCode))
        {
            Trigger();
            return;

        }

        // if (_asset.IsDown())
        // {
        //     Trigger();
        //     return;
        // }
    }

    private void Trigger()
    {
        _onPressed?.Invoke();
    }
}