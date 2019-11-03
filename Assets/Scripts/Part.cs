using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour {

    private int id;
    private bool state;
    private Vector2Int coords;
    private bool isActive; // state cannot be changed externally
    private bool isSelected;

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

    public bool IsActive () {
        return isActive;
    }

    public void SetIsActive (bool isActive) {
        this.isActive = isActive;
    }

    public bool IsSelected () {
        return isSelected;
    }

    public void SetSelected (bool isSelected) {
        this.isSelected = isSelected;
        OnSelectUpdate ();
        GetSim().ToggleSelected(this);
    }

    public abstract void OnStateUpdate ();

    public abstract void OnSelectUpdate ();

    public override string ToString () {
        return "Type: " + this.GetType () + "| Part Id: " + id;
    }

    public SimulationManager GetSim () {
        return SimulationManager.sim ();
    }

}