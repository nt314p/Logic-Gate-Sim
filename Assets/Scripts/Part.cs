using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour {

    private int id;
    private bool state;
    private Vector2Int coords;

    public int GetId () {
        return id;
    }

    public void SetId (int id) {
        this.id = id;
    }

    public bool GetState () {
        return state;
    }

    public void SetState (bool state) {
        this.state = state;
        OnStateUpdate ();
    }

    public Vector2Int GetCoords () {
        return coords;
    }

    public void SetCoords (Vector2Int coords) {
        this.coords = coords;
    }

    public abstract void OnStateUpdate ();

    public override string ToString() {
        return "Type: " + this.GetType() +"| Part Id: " + id;
    }

}