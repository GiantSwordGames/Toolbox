using System;
using System.Collections;
using System.Collections.Generic;
using JamKit;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _textMeshPro;
    [Space]
    [SerializeField] private float _lifetime = 1;
    [Space]
    [SerializeField] private AnimationCurve _yPositionOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] private float _positionMultiplier = 1;
    [Space]
    [SerializeField] private AnimationCurve _scaleOverLifeTime = AnimationCurve.Linear(0,0,1,0);
    [SerializeField] private float _scaleMultiplier = 1;
    [Space]
    [SerializeField] private float _colorSpeedMultiplier = 1;
    [SerializeField] private Gradient _colorOverLifeTime;
    [SerializeField] private AnimationCurve _alphaOverLifeTime = AnimationCurve.Linear(0,1,1,1);
    [Space]
    [SerializeField] private bool _autoDestroy = true;
    [SerializeField] private bool _loop = false;

    private Vector3 positionOffset;
    private Vector3 scaleOffset;
    float _lerp = 0;


    private void Start()
    {

        gameObject.AddComponent<FaceCamera>();
    }

    public void Setup  (string text)
    {
        if (_textMeshPro != null)
        {
            _textMeshPro.text = text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _lerp += Time.deltaTime / _lifetime ;
        if (_lerp > 1)
        {
            if (_loop)
            {
                _lerp %= 1;
            }
            else
            {

                _lerp = 1;
                if (_autoDestroy)
                {
                    Destroy(gameObject);
                }
            }
        }

        Color color = _colorOverLifeTime.Evaluate(_lerp * _colorSpeedMultiplier);
        color.a *= _alphaOverLifeTime.Evaluate(_lerp);
        _textMeshPro.color = color;
        
        transform.localPosition -= positionOffset;
        transform.localScale -= scaleOffset;
        
        scaleOffset = Vector3.one * (_scaleOverLifeTime.Evaluate(_lerp) * _scaleMultiplier);
        positionOffset.y = _yPositionOverLifeTime.Evaluate(_lerp)*_positionMultiplier;

        transform.localPosition += positionOffset;
        transform.localScale +=   scaleOffset;
    }

    [Button]
    public void Reset()
    {
        _lerp = 0;
    }

    [Button]
    public void Instantiate()
    {
        RuntimeEditorHelper.SmartInstantiate( gameObject);
    }
    
    [Button]
    public void Instantiate(string text, Vector3 position)
    {
        GameObject instantiate = RuntimeEditorHelper.SmartInstantiate( gameObject);
        instantiate.GetComponent<PopUpText>().Setup(text);
    }
}
