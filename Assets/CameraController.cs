using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	// If you think that camera is flickering try to use rigidbody Interpolate for target
	[SerializeField] private Transform target;
	[SerializeField] private float angle;
	[SerializeField] private float distance;
	[SerializeField] private float focusRadius;
	[SerializeField] private float cameraSpeed;
	private Vector3 _lookDistance;
	private Vector3 _focusPoint;
	private Vector3 _lastFocusPoint;
	private float _time = 0;
	private void Start()
	{
		_focusPoint = target.position;
		_lastFocusPoint = target.position;
		transform.position = target.position + _lookDistance;
		_lookDistance = new Vector3(0,distance*Mathf.Sin(angle*Mathf.Deg2Rad),-1*distance*Mathf.Cos(angle*Mathf.Deg2Rad));
	}

	private void LateUpdate()
	{
		UpdateFocusPoint();
		UpdateTransform();
	}

	private void UpdateFocusPoint()
	{
		_focusPoint = target.position;
		float distance = Vector3.Distance(_focusPoint, _lastFocusPoint);
		if ( distance>= focusRadius)
		{
			_lastFocusPoint = Vector3.Lerp(_lastFocusPoint, _focusPoint, focusRadius / distance);
		}
		else
		{
			_lastFocusPoint = Vector3.Lerp(_focusPoint, _focusPoint, Time.unscaledDeltaTime);
		}
		
	}

	private void UpdateTransform()
	{
		_time = Time.deltaTime;
		
		transform.position = Vector3.Lerp(transform.position,_lastFocusPoint+_lookDistance,_time*cameraSpeed);
		transform.rotation = Quaternion.Euler(angle, 0, 0);
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;

		Gizmos.DrawSphere(_focusPoint,.1f);
		
		Gizmos.color = Color.red;

		Gizmos.DrawSphere(_lastFocusPoint,.1f);

	}
}