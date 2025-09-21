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

	[SerializeField] private float _duration = 0;
	bool _isOn;

	[Range(0, 1)] [SerializeField] private float _control;
	private Coroutine _lerpRoutine;

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
		Vector3 previousOffset = _positionOffset * _lerp;
		transform.localPosition -= previousOffset;
		transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(_rotationOffset * _lerp));
		_lerp = newValue;
		Vector3 newOffset = _positionOffset * _lerp;
		transform.localPosition += newOffset;
		transform.localRotation *= (Quaternion.Euler(_rotationOffset * _lerp));
	}

	public void TriggerOn()
	{
		_isOn = true;
		SetLerp(1);
	}

	public void TriggerOff()
	{
		_isOn = false;
		SetLerp(0);
	}

	public void TweenOn()
	{
		_isOn = true;
		_lerpRoutine.KillAsyncRoutineAsNeeded();
		_lerpRoutine = AsyncHelper.LerpRoutine(_duration, (l) => SetLerp(l));
	}

	public void TweenOff()
	{
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
