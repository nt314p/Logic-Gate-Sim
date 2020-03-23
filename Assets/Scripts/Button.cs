using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Part {

    private SpriteRenderer toggleSR;
    // Start is called before the first frame update
    void Start () {
        // get the toggle sprite renderer
        toggleSR = GameObject.Find ("/" + this.gameObject.name + "/Toggle").GetComponent<SpriteRenderer> ();
        Debug.Log (toggleSR);
    }

    // Update is called once per frame
    void Update () {

    }

    void OnMouseDown () {
        if (Input.GetKey (KeyCode.LeftControl)) {
            SetState (true);
            GetSim ().GetCircuit ().CalculateStateId (GetId ());
        } else {
            Debug.Log ("clicked " + this.ToString ());
        }
    }

    void OnMouseUpAsButton () {
        SetState (false); // untoggle button
    }

    public override void OnSelectUpdate () {

    }

    public override void OnStateUpdate () {
        if (GetState ()) {
            toggleSR.color = new Color (0f, 0.7882353f, 0.0902f); // bright green
        } else {
            toggleSR.color = new Color (0.04705883f, 0.454902f, 0.1137255f); // dark green
        }
    }
}