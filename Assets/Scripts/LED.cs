using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : Component {

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        SetState (false);
        UpdateColor ();
    }

    // Update is called once per frame
    void Update () {

    }

    public void UpdateColor () {
        if (GetState ()) {
            sr.color = new Color (0f, 0.79f, 0.09f);
        } else {
            sr.color = new Color (0f, 0.494f, 0.0588f);
        }
    }

    void OnMouseDown () {
        if (Input.GetKey (KeyCode.LeftControl)) {
            SetState (!GetState ());
            FindObjectOfType<WireManager> ().SetAllOfId (GetId (), GetState ());
        }
        sr.color = new Color (0.67f, 0.89f, 0f);
        sr.sortingOrder = 1;
    }

    void OnMouseUp () {
        UpdateColor ();
        sr.sortingOrder = 0;
    }

    public override void OnStateUpdate () {

    }
}