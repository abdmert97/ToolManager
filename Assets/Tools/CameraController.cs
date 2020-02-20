using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoBehaviour
	{
		// If you think that camera is flickering try to use rigidbody Interpolate for target
		public Transform target;
		public float angle;
		public float distance;
		public float focusRadius;
		public float cameraSpeed;
		[Tooltip("When target objects rotation changes camera will also rotates")]	[HideInInspector]
		public bool relativeRotation;
		public Vector3 defaultRotation;

		public Text text;
		[HideInInspector]
		public bool cullObjectFrontofTarget;
		[Range(0,255)][HideInInspector]
		public int transparency = 128;
		[HideInInspector]
		public Material transparentMaterial = null;
		[HideInInspector]
		public bool fixedCamera = false;
		[HideInInspector]
		public bool autoCameraSpeed = false;
		[HideInInspector]
		public bool stopCamera = false;
		[HideInInspector]
		public bool lookAlwaysTarget;
		[HideInInspector]
		public bool lerpAngle = false;
		
		private Vector3 _lookDistance;
		private Vector3 _focusPoint;
		private Vector3 _lastFocusPoint;
		private float _time = 0;
		private List<Renderer> _disabledRenderers = new List<Renderer>();
		private List<Material> _disabledMaterials = new List<Material>();
		private Vector3 _hitNormal;
		private Camera _camera;
		private float _defaultCameraSpeed;
		private void Awake()
		{
			if(transparentMaterial == null)
			{
				cullObjectFrontofTarget = false;
				//transparentMaterial = new Material(Shader.Find("Transparent/Diffuse"));
				//transparentMaterial.color = new Color32(255, 255, 255, (byte)transparency);
			}
		}

		private void Start()
		{
			_defaultCameraSpeed = cameraSpeed;
			var position = target.position;
			_focusPoint = position;
			_lastFocusPoint = position;
			transform.position = position + _lookDistance;
			_lookDistance = new Vector3(0,distance*Mathf.Sin(angle*Mathf.Deg2Rad),-1*distance*Mathf.Cos(angle*Mathf.Deg2Rad));
			_camera = Camera.main;
			text.text = target.name;
		}

		
		private void OnValidate()
		{
			if(!lookAlwaysTarget)
				_lookDistance = new Vector3(0,distance*Mathf.Sin(angle*Mathf.Deg2Rad),-1*distance*Mathf.Cos(angle*Mathf.Deg2Rad));
			else
			{
				Vector3 rotation = Vector3.right * angle + defaultRotation;
				
				_lookDistance = new Vector3(0,distance*Mathf.Sin(angle*Mathf.Deg2Rad),-1*distance*Mathf.Cos(angle*Mathf.Deg2Rad));
			}
			distance = distance < 0 ? 0 : distance;
		}
	
		private void LateUpdate()
		{
			if (stopCamera) return;
			UpdateFocusPoint();
			UpdateTransform();
		}

		private void FixedUpdate()
		{
			if(cullObjectFrontofTarget)
				CheckVisibility();
		}

		private void CheckVisibility()
		{
			if (Physics.Raycast(transform.position, target.position - transform.position, out var hit))
			{
				if (!hit.transform.Equals(target))
				{
					Renderer renderer = hit.transform.GetComponent<Renderer>();
					if (!_disabledRenderers.Contains(renderer))
					{
						_disabledRenderers.Add(renderer);
						_disabledMaterials.Add(renderer.sharedMaterial);
					}
					renderer.sharedMaterial = transparentMaterial;
				}
				else
				{
					for (int i = 0; i < _disabledRenderers.Count; i++)
					{
						_disabledRenderers[i].sharedMaterial = _disabledMaterials[i];
					}
					_disabledRenderers.Clear();
					_disabledMaterials.Clear();
				}
			}
		}


		private void UpdateFocusPoint()
		{
			_focusPoint = target.position;
		
			float distance = Vector3.Distance(_focusPoint, _lastFocusPoint);
			if ( distance>= focusRadius)
			{
				_lastFocusPoint = _focusPoint;
			}
			else
			{
				//	_lastFocusPoint = Vector3.Lerp(_focusPoint, _focusPoint, Time.unscaledDeltaTime);
			}
		}

		private void UpdateTransform()
		{
			_time = Time.deltaTime;
			
			UpdatePosition();
			
			UpdateRotation();
		}

		private void UpdatePosition()
		{
			if (autoCameraSpeed)
			{
				Vector2 isVisible =	_camera.WorldToViewportPoint(target.position);
			  
				if (isVisible.x> 0.75f || isVisible.y > 0.75f ||isVisible.x< 0.25f || isVisible.y < 0.25f)
				{
					cameraSpeed += Time.deltaTime;
				}
				else
				{
					// cameraSpeed = defaultCameraSpeed;
				}
			}
			
			Vector3 distance = _lastFocusPoint + _lookDistance - transform.position;
			if (!fixedCamera)
			{
				distance = distance * cameraSpeed*Time.deltaTime;
			}
			
			//transform.position = Vector3.Lerp(transform.position, _lastFocusPoint + _lookDistance, _time * cameraSpeed);
			transform.position += distance;
		}

		private void UpdateRotation()
		{
			Vector3 rotation = defaultRotation + Vector3.right * angle;
			if (!relativeRotation)
			{
				transform.rotation = !lerpAngle
					? Quaternion.Euler(rotation)
					: Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), _time * cameraSpeed);
			}
			else
			{
				if (Physics.Raycast(transform.position - _lookDistance, Vector3.down, out var hit))
				{
					_hitNormal = hit.normal;
					float normalAngle = 90 - Mathf.Rad2Deg * Mathf.Atan2(_hitNormal.y, hit.normal.z);
					rotation += normalAngle * Vector3.right;
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), _time * cameraSpeed);
				}
				else
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), _time * cameraSpeed);
				}
			}
		}


		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;

			Gizmos.DrawSphere(_focusPoint,.1f);
		
			Gizmos.color = Color.red;

			Gizmos.DrawSphere(_lastFocusPoint,.1f);

			var position = target.position;
			Gizmos.DrawLine(position,position+_hitNormal*5);

		}
	}
}
