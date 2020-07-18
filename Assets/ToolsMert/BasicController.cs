using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicController : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField, Min(0.1f)] private float _speedMultiplier;
    [SerializeField, Min(0.1f)] private float _rotationSpeedMultiplier;
    // Start is called before the first frame update
    private Camera _camera;
    private Vector3 speedVector;
    void Start()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal")*_speedMultiplier;
        float vertical = Input.GetAxis("Vertical")*_speedMultiplier;
        if (Input.GetMouseButton(0))
        {
            Vector2 view = _camera.ScreenToViewportPoint(Input.mousePosition);
            view -= Vector2.one*0.5f;
            view *= 50;
            horizontal = view.x;
            vertical = view.y;
        }
        
        speedVector = new Vector3(horizontal,_rigidbody.velocity.y,vertical);
        if (Input.GetKey(KeyCode.Space))
        {
            speedVector+= Vector3.up*_speedMultiplier*Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
           transform.Rotate(Vector3.up*_rotationSpeedMultiplier);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.down*_rotationSpeedMultiplier);
        }

    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = speedVector;
    }
}
