using System;
using System.Collections;
using System.Collections.Generic;
using JamKit;
using NaughtyAttributes;
using UnityEngine;

public class MoveTransform : MonoBehaviour
{
	    [SerializeField] private  Vector3 _offset = Vector3.right*10; 
		[DisableSerializedField]	[SerializeField] private  float _lerp; 
	    
	    [Range(0,1)]
	    [SerializeField] private  float _control;

	    private void OnValidate()
	    {
		    SetLerp(_control);
	    }

	    // Update is called once per frame
	    public void SetLerp(float newValue)
	    {
		    Vector3 previousOffset = _offset * _lerp;
		    transform.localPosition -= previousOffset;
		    _lerp = newValue;
		    Vector3 newOffset = _offset * _lerp;
		    transform.localPosition += newOffset;
	    }
}
