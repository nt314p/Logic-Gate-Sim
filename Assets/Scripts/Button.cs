using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Part {

    private SpriteRenderer toggleSR;
    
    void Awake () {
        // get the toggle sprite renderer
        toggleSR = GameObject.Find ("/" + this.gameObject.name + "/Toggle").GetComponent<SpriteRenderer> ();

        this.State = false;
        this.Active = false;
    }

    // Update is called once per frame
    void Update () {

    }

    void OnMouseDown () {
        if (Input.GetKey (KeyCode.LeftControl)) {
            this.State = true;
            GetSim ().GetCircuit ().CalculateStateId (this.Id);
        } else {
            Debug.Log ("clicked " + this.ToString ());
        }
    }

    void OnMouseUpAsButton () {
        State = false; // untoggle button
        GetSim ().GetCircuit ().CalculateStateId (this.Id); // fix this sketchy code
        // perhaps add a "updated" boolean that is set if the state has been updated
        // then the Circuit iterates through active parts and sees if any parts have been updated
        // if so, then update the part (and its corresponding ids)
    }

    public override void OnSelectUpdate () {

    }

    public override void OnStateUpdate () {
        toggleSR.color = State ? this.ActiveColor : this.InactiveColor;
    }
}