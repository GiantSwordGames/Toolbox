using System;
using System.Collections;
using System.Collections.Generic;
using JamKit;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveTransform : MonoBehaviour
{
	[FormerlySerializedAs("_offset")] [SerializeField]
	private Vector3 _positionOffset = Vector3.right * 10;

	[SerializeField] private Vector3 _rotationOffset = Vector3.zero;

	[DisableSerializedField] [SerializeField]
	private float _lerp;
	[FormerlySerializedAs("_previousLerp")] [DisableSerializedField] [SerializeField]
	private float _previousEvaluation;

	[SerializeField] private float _duration = 0;
	bool _isOn;

	[Range(0, 1)] [SerializeField] private float _control;
	private Coroutine _lerpRoutine;


	[SerializeField] EasingFunction.Ease _easing = EasingFunction.Ease.Linear;
	
	public float lerp
	{
		get => _lerp;
		set => SetLerp(value);
	}

	public bool isOn => _isOn;

	private void OnValidate()
	{
		SetLerp(_control);
	}
	

	private void Start()
	{
		if (lerp == 1)
		{
			_isOn = true;
		}
	}

	// Update is called once per frame
	public void SetLerp(float newValue)
	{
		transform.localPosition -= _positionOffset * _previousEvaluation;
		transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(_rotationOffset * _previousEvaluation));
		_lerp = newValue;
		float evaluatedLerp = EasingFunction.GetEasingFunction(_easing)(0, 1, _lerp);
		Vector3 newOffset = _positionOffset * evaluatedLerp;
		transform.localPosition += newOffset;
		transform.localRotation *= (Quaternion.Euler(_rotationOffset * evaluatedLerp));
		_previousEvaluation = evaluatedLerp;
	}

	public void TriggerOn()
	{
		if(enabled== false)
			return;

		_isOn = true;
		SetLerp(1);
	}

	public void TriggerOff()
	{
		if(enabled== false)
			return;

		_isOn = false;
		SetLerp(0);
	}

	public void TweenOn()
	{
		if(enabled== false)
			return;

		_isOn = true;
		_lerpRoutine.KillAsyncRoutineAsNeeded();
		_lerpRoutine = AsyncHelper.LerpRoutine(_duration, (l) => SetLerp(l));
	}

	public void TweenOff()
	{
		if(enabled== false)
			return;

		_isOn = false;
		_lerpRoutine.KillAsyncRoutineAsNeeded();
		_lerpRoutine = AsyncHelper.LerpRoutine(_duration, (l) => SetLerp(1 - l));
	}

	[Button]
	public void Trigger()
	{
		
		TweenOn();
	}

	public void ToggleTween()
	{
		if(enabled== false)
			return;
		
		if (_isOn)
		{
			TweenOff();
		}
		else
		{
			TweenOn();
		}

	}

}
