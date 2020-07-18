using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraNet : MonoBehaviour
{

	[SerializeField] private Transform focus = default;

	[SerializeField, Range(1f, 20f)] private float distance = 5f;

	[Min(0f)] private float _focusRadius = 5f;

	[SerializeField][Range(0f, 1f)] private float focusCentering = 0.5f;

	[SerializeField][Range(1f, 360f)] private float rotationSpeed = 90f;

	[SerializeField][Range(-89f, 89f)] private float minVerticalAngle = -45f, maxVerticalAngle = 45f;

	[SerializeField][Min(0f)] private float alignDelay = 5f;

	[SerializeField][Range(0f, 90f)] private float alignSmoothRange = 45f;

	[SerializeField] private LayerMask obstructionMask = -1;

	private Camera _regularCamera;

	private Vector3 _focusPoint, _previousFocusPoint;

	private Vector2 _orbitAngles = new Vector2(45f, 0f);

	private float _lastManualRotationTime;

	Vector3 CameraHalfExtends
	{
		get
		{
			Vector3 halfExtends;
			halfExtends.y = _regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * _regularCamera.fieldOfView);
			halfExtends.x = halfExtends.y * _regularCamera.aspect;
			halfExtends.z = 0f;
			return halfExtends;
		}
	}

	void OnValidate()
	{
		if (maxVerticalAngle < minVerticalAngle)
		{
			maxVerticalAngle = minVerticalAngle;
		}
	}

	void Awake()
	{
		_regularCamera = GetComponent<Camera>();
		_focusPoint = focus.position;
		transform.localRotation = Quaternion.Euler(_orbitAngles);
	}

	void LateUpdate()
	{
		UpdateFocusPoint();
		Quaternion lookRotation;
		if (ManualRotation() || AutomaticRotation())
		{
			ConstrainAngles();
			lookRotation = Quaternion.Euler(_orbitAngles);
		}
		else
		{
			lookRotation = transform.localRotation;
		}

		var lookDirection = lookRotation * Vector3.forward;
		var lookPosition = _focusPoint - lookDirection * distance;

		var rectOffset = lookDirection * _regularCamera.nearClipPlane;
		var rectPosition = lookPosition + rectOffset;
		var castFrom = focus.position;
		var castLine = rectPosition - castFrom;
		var castDistance = castLine.magnitude;
		var castDirection = castLine / castDistance;

		if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out var hit, lookRotation, castDistance, obstructionMask))
		{
			rectPosition = castFrom + castDirection * hit.distance;
			lookPosition = rectPosition - rectOffset;
		}

		transform.SetPositionAndRotation(lookPosition, lookRotation);
	}

	void UpdateFocusPoint()
	{
		_previousFocusPoint = _focusPoint;
		Vector3 targetPoint = focus.position;
		if (_focusRadius > 0f)
		{
			float distance = Vector3.Distance(targetPoint, _focusPoint);
			if (distance > _focusRadius)
			{
				_focusPoint = Vector3.Lerp(
					targetPoint, _focusPoint, _focusRadius / distance
				);
			}

			if (distance > 0.01f && focusCentering > 0f)
			{
				_focusPoint = Vector3.Lerp(
					targetPoint, _focusPoint,
					Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime)
				);
			}
		}
		else
		{
			_focusPoint = targetPoint;
		}
	}

	bool ManualRotation()
	{
		return false;
		Vector2 input = new Vector2(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera")
		);
		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e)
		{
			_orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
			_lastManualRotationTime = Time.unscaledTime;
			return true;
		}

		return false;
	}

	bool AutomaticRotation()
	{
		if (Time.unscaledTime - _lastManualRotationTime < alignDelay)
		{
			return false;
		}

		Vector2 movement = new Vector2(
			_focusPoint.x - _previousFocusPoint.x,
			_focusPoint.z - _previousFocusPoint.z
		);
		float movementDeltaSqr = movement.sqrMagnitude;
		if (movementDeltaSqr < 0.0001f)
		{
			return false;
		}

		float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
		float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(_orbitAngles.y, headingAngle));
		float rotationChange =
			rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
		if (deltaAbs < alignSmoothRange)
		{
			rotationChange *= deltaAbs / alignSmoothRange;
		}
		else if (180f - deltaAbs < alignSmoothRange)
		{
			rotationChange *= (180f - deltaAbs) / alignSmoothRange;
		}

		_orbitAngles.y =
			Mathf.MoveTowardsAngle(_orbitAngles.y, headingAngle, rotationChange);
		return true;
	}

	void ConstrainAngles()
	{
		_orbitAngles.x =
			Mathf.Clamp(_orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (_orbitAngles.y < 0f)
		{
			_orbitAngles.y += 360f;
		}
		else if (_orbitAngles.y >= 360f)
		{
			_orbitAngles.y -= 360f;
		}
	}

	static float GetAngle(Vector2 direction)
	{
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}
}