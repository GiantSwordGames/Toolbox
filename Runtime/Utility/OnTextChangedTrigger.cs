using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OnTextChangedTrigger : MonoBehaviour
{
    
    [SerializeField] private TMP_Text _text;
    [SerializeField] private UnityEvent _onChanged;

    private string _previous;

    public UnityEvent onChanged => _onChanged;

    void Start()
    {
        _previous = _text.text;
    }

    // Update is called once per frame
    void Update()
    {
        if(_previous != _text.text)
        {
            _previous = _text.text;
            Trigger();
        }
    }

    [Button]
    private void Trigger()
    {
        onChanged?.Invoke();
    }
}
