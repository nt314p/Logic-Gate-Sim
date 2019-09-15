using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : Part {

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        SetState (false);
        SetIsActive(false);
        UpdateColor ();
    }

    public void UpdateColor () {
        if (GetState ()) {
            sr.color = new Color (0f, 0.79f, 0.09f);
        } else {
            sr.color = new Color (0f, 0.494f, 0.0588f);
        }
    }

    void OnMouseDown () {
        Debug.Log ("clicked " + this.ToString ());
        sr.color = new Color (0.67f, 0.89f, 0f);
    }

    void OnMouseUp () {
        UpdateColor ();
    }

    public override void OnStateUpdate () {
        UpdateColor ();
    }
}