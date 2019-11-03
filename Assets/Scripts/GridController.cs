using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridController : MonoBehaviour {
    public static int width = 30;
    public static int height = 30;
    private Material gridMat;
    private GameObject gridPlane;

    void Start () {
        gridPlane = this.gameObject;
        gridMat = gridPlane.GetComponent<Renderer> ().material;
    }

    // Update is called once per frame
    void Update () {
        gridMat.mainTextureScale = new Vector2 (width, height);
        gridPlane.transform.localScale = new Vector3 (2.34375f, 2.34375f, 1);
        // gridPlane.transform.position = new Vector3 (width * 0.5f - 0.5f, height * 0.5f - 0.5f, 0);
        gridPlane.transform.position = new Vector3 (-0.5f, height - 0.5f, 0);

    }

    void OnMouseDown () {
        Debug.Log ("MDPLANE!");
        SimulationManager.sim ().ClearSelected ();
    }

    public void SetWidth (int width) {
        GridController.width = width;
    }

    public void SetHeight (int height) {
        GridController.height = height;
    }
}