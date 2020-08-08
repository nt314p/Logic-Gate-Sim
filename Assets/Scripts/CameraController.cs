using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private const float ZoomScale = 0.5f;
    private const float InitialZoom = 10;
    private const float MinZoom = 1.2f;
    private const float MaxZoom = 15;
    private const float DragSpeed = 2;
    private float _zoom;
    private Vector3 _dragOrigin, _oldPosition;
    [SerializeField] private Camera _mainCamera;

    private void Start()
    {
        _mainCamera.orthographicSize = InitialZoom;
        _zoom = InitialZoom;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _dragOrigin = Input.mousePosition;
            _oldPosition = transform.position;
        }

        if (Input.GetMouseButton(1))
        {
            var position = _mainCamera.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
            transform.position = _oldPosition - position * _zoom * DragSpeed;
        }

        var scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollInput < 0 && _zoom < MaxZoom)
        {
            ZoomCamera(_mainCamera.ScreenToWorldPoint(Input.mousePosition), -ZoomScale);
        }
        if (scrollInput > 0 && _zoom > MinZoom)
        {
            ZoomCamera(_mainCamera.ScreenToWorldPoint(Input.mousePosition), ZoomScale);
        }
        
        _mainCamera.orthographicSize = _zoom;
    }

    public float GetZoom()
    {
        return _zoom;
    }

    private void ZoomCamera(Vector3 target, float scale)
    {
        var multiplier = (1.0f / _zoom * scale);
        transform.position += (target - transform.position) * multiplier; // zoom in at mouse pointer effect
        _zoom -= scale;
    }
}