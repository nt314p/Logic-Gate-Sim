using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGridController : MonoBehaviour {
    public int width;
    public int height;
    private Material gridMat;
    private GameObject gridPlane;

    void Start () {
        width = 50;
        height = 50;
        gridPlane = this.gameObject;
        gridMat = gridPlane.GetComponent<Renderer> ().material;
    }

    // Update is called once per frame
    void Update () {
        gridMat.mainTextureScale = new Vector2(width, height);
        gridPlane.transform.localScale = new Vector3(width/10, 1, height/10);
        gridPlane.transform.position = new Vector3(width * 0.5f - 0.5f, height * 0.5f - 0.5f, 0);
    }

    public void SetWidth (int width) {
        this.width = width;
    }

    public void SetHeight (int height) {
        this.height = height;
    }
}