using Boo.Lang;
using UnityEngine;

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
		[Tooltip("When target objects rotation changes camera will also rotates")]
		public bool relativeRotation; 
		public bool cullObjectFrontofTarget; 
		private Vector3 _lookDistance;
		private Vector3 _focusPoint;
		private Vector3 _lastFocusPoint;
		private float _time = 0;
		public Material transparent = null;
		private List<Renderer> _disabledRenderers = new List<Renderer>();
		private List<Material> _disabledMaterials = new List<Material>();

		private Vector3 hitNormal;


		private void Awake()
		{
			if(transparent == null)
			{
				transparent = new Material(Shader.Find("Transparent/Diffuse"));
				transparent.color = new Color32(255, 255, 255, 128);
			}

		}

		private void Start()
		{
		
			var position = target.position;
			_focusPoint = position;
			_lastFocusPoint = position;
			transform.position = position + _lookDistance;
			_lookDistance = new Vector3(0,distance*Mathf.Sin(angle*Mathf.Deg2Rad),-1*distance*Mathf.Cos(angle*Mathf.Deg2Rad));
		}

	
		private void OnValidate()
		{
			_lookDistance = new Vector3(0,distance*Mathf.Sin(angle*Mathf.Deg2Rad),-1*distance*Mathf.Cos(angle*Mathf.Deg2Rad));
			distance = distance < 0 ? 0 : distance;
		}
	
		private void LateUpdate()
		{
			UpdateFocusPoint();
			UpdateTransform();
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
					renderer.sharedMaterial = transparent;
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
			if(!relativeRotation)
				transform.rotation = Quaternion.Euler(angle, 0, 0);
			else
			{
			
				if (Physics.Raycast(transform.position - _lookDistance, Vector3.down,out var hit))
				{
					hitNormal = hit.normal;
					float normalAngle = 90-Mathf.Rad2Deg * Mathf.Atan2(hitNormal.y, hit.normal.z);
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angle+normalAngle, 0, 0),_time*cameraSpeed); 
				
				}
				else
				{
					transform.rotation = Quaternion.Euler(angle, 0, 0);
				}
			}
		}

	
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;

			Gizmos.DrawSphere(_focusPoint,.1f);
		
			Gizmos.color = Color.red;

			Gizmos.DrawSphere(_lastFocusPoint,.1f);
		
			Gizmos.DrawLine(target.position,target.position+hitNormal*5);

		}
	}
}