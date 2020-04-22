using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private float zoom;
    private readonly float zoomScale = 0.5f;
    private readonly float initZoom = 10;
    private readonly float minZoom = 1.2f;
    private readonly float maxZoom = 15;
    private Vector3 targetPos;
    private Vector3 dragOrigin, oldPos;

    void Start () {
        Camera.main.orthographicSize = initZoom;
        zoom = initZoom;
        targetPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown (1)) {
            dragOrigin = Input.mousePosition;
            oldPos = transform.position;
        }

        if (Input.GetMouseButton (1)) {
            //Camera.main.ScreenToViewportPoint (Input.mousePosition - dragOrigin);
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = oldPos + -pos * zoom;
            targetPos += move - transform.position;
            transform.position = move;
            
        }

        float input = Input.GetAxis ("Mouse ScrollWheel");
        if (input < 0 && zoom < maxZoom) {
            ZoomCamera (Camera.main.ScreenToWorldPoint (Input.mousePosition), -zoomScale);
        }
        if (input > 0 && zoom > minZoom) {
            ZoomCamera (Camera.main.ScreenToWorldPoint (Input.mousePosition), zoomScale);
        }

        transform.position = Vector3.Lerp (transform.position, targetPos, 0.6f);
        Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, zoom, 0.6f);
    }

    public float GetZoom() {
        return zoom;
    }

    private void ZoomCamera (Vector3 target, float scale) {
        float multiplier = (1.0f / zoom * scale);

        targetPos = targetPos + (target - targetPos) * multiplier;
        zoom -= scale;
    }
}