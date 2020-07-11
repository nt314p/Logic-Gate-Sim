using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Part : MonoBehaviour {

    private int id;
    private bool state;
    private Vector2Int coords;
    private bool isActive; // if true state cannot be changed externally
    private bool isSelected;
    [SerializeField]
    protected Color ActiveColor; // (0f, 0.7882353f, 0.0902f) bright green
    [SerializeField]
    protected Color InactiveColor; // (0.04705883f, 0.454902f, 0.1137255f) dark green
    [SerializeField]
    protected Color SelectedColor; // (0.4478532f, 0.8867924f, 0f) another bright green

    public int Id {
        get { return this.id; }
        set { this.id = value; }
    }

    public bool State {
        get { return this.state; }
        set { this.state = value; OnStateUpdate (); }
    }

    public Vector2Int Coords {
        get { return this.coords; }
        set { this.coords = value; }
    }

    public bool Active {
        get { return this.isActive; }
        set { this.isActive = value; }
    }

    public bool Selected {
        get { return this.isSelected; }
        set {
            this.isSelected = value;
            OnSelectUpdate ();
            GetSim ().ToggleSelected (this); // not sure about this line...
        }
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