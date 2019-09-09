using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        gridMat.mainTextureScale = new Vector2(width, height);
        gridPlane.transform.localScale = new Vector3(width/10f, 1, height/10f);
        gridPlane.transform.position = new Vector3(width * 0.5f - 0.5f, height * 0.5f - 0.5f, 0);
    }

    public void SetWidth (int width) {
        GridController.width = width;
    }

    public void SetHeight (int height) {
        GridController.height = height;
    }
}