using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPressed;
    [SerializeField]  [FormerlySerializedAs("inputAction")] private InputActionReference _inputAction;
    [SerializeField] KeyCode _keyCode = KeyCode.None;

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

    void Update()
    {
        if (_inputAction != null && _inputAction.action.triggered)
        {
            Trigger();
        }

        if (Input.GetKeyDown(_keyCode))
        {
            Trigger();
        }
    }

    private void Trigger()
    {
        _onPressed?.Invoke();
    }
}