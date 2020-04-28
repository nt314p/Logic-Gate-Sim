using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour {

    private int id;
    private bool state;
    private Vector2Int coords;
    private bool isActive; // state cannot be changed externally
    private bool isSelected;
    public readonly static Color colorActive = new Color (0f, 0.7882353f, 0.0902f); // bright green
    public readonly static Color colorInactive = new Color (0.04705883f, 0.454902f, 0.1137255f); // dark green
    public readonly static Color colorSelected = new Color (0.4478532f, 0.8867924f, 0f); // another bright green
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
        GetSim ().ToggleSelected (this);
    }

    public abstract void OnStateUpdate ();

    public abstract void OnSelectUpdate ();

    public override string ToString () {
        return "Type: " + this.GetType () + "; Part Id: " + id;
    }

    public SimulationManager GetSim () {
        return SimulationManager.sim ();
    }

}