using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : Part {

    private SpriteRenderer sr;

    void Awake () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        this.State = false;
        this.Active = false;
        UpdateColor ();
    }

    public void UpdateColor () {
        sr.color = this.State ? this.ActiveColor : this.InactiveColor;
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

    public override void OnSelectUpdate () {
        throw new System.NotImplementedException ();
    }
}