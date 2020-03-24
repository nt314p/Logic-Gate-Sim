using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Part {

    private SpriteRenderer toggleSR;
    
    void Awake () {
        // get the toggle sprite renderer
        toggleSR = GameObject.Find ("/" + this.gameObject.name + "/Toggle").GetComponent<SpriteRenderer> ();

        SetState (false);
        SetIsActive(true);
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
        GetSim ().GetCircuit ().CalculateStateId (GetId ()); // fix this sketchy code
        // perhaps add a "updated" boolean that is set if the state has been updated
        // then the Circuit iterates through active parts and sees if any parts have been updated
        // if so, then update the part (and its corresponding ids)
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