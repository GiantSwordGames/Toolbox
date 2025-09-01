using System;
using System.Collections;
using System.Collections.Generic;
using JamKit;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveTransform : MonoBehaviour
{
	    [FormerlySerializedAs("_offset")] [SerializeField] private  Vector3 _positionOffset = Vector3.right*10; 
	    [SerializeField] private  Vector3 _rotationOffset = Vector3.zero; 
		[DisableSerializedField]	[SerializeField] private  float _lerp; 
	    
	    [Range(0,1)]
	    [SerializeField] private  float _control;

	    public float lerp
	    {
		    get => _lerp;
		    set => SetLerp(value);
	    }

	    private void OnValidate()
	    {
		    SetLerp(_control);
	    }

	    // Update is called once per frame
	    public void SetLerp(float newValue)
	    {
		    Vector3 previousOffset = _positionOffset * _lerp;
		    transform.localPosition -= previousOffset;
		    transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(_rotationOffset * _lerp));
		    _lerp = newValue;
		    Vector3 newOffset = _positionOffset * _lerp;
		    transform.localPosition += newOffset;
		    transform.localRotation *=(Quaternion.Euler(_rotationOffset * _lerp));
	    }

	    public void TriggerOn()
	    {
		    SetLerp(1);
	    }
	    
	    public void TriggerOff()
	    {
		    SetLerp(0);
	    }
	    
	    public void TweenOn(float duration)
	    {
		    AsyncHelper.LerpRoutine(duration, (l) => SetLerp(l));
	    }
	    public void TweenOff(float duration)
	    {
		    AsyncHelper.LerpRoutine(duration, (l) => SetLerp(1-l));
	    }


}
