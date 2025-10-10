using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _from;
    [SerializeField] private TextMeshProUGUI _to;

    void Start()
    {
        if (_to == null)
        {
            _to = GetComponent<TextMeshProUGUI>();
        }
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_to.text != _from.text)
        {
            _to.text = _from.text;
        }
    }
}
