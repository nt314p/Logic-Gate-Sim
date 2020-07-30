using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public abstract class Part
{
    private int id;
    private bool state;
    private bool hasStateUpdate;
    private bool isSelected;
    private bool hasSelectedUpdate;
    private Vector2Int coords;
    private bool isActive; // if true state cannot be changed externally

    public Part(bool isActive)
    {
        this.isActive = isActive;
    }

    public int Id
    {
        get => this.id;
        set => this.id = value;
    }

    public bool State
    {
        get => this.state;
        set
        {
            if (this.state != value) hasStateUpdate = true;
            this.state = value;
        }
    }

    public Vector2Int Coords
    {
        get => this.coords;
        set => this.coords = value;
    }

    public bool Active
    {
        get => this.isActive;
        //set => this.isActive = value;
    }

    public bool Selected
    {
        get { return this.isSelected; }
        set
        {
            if (this.isSelected != value) hasSelectedUpdate = true;
            this.isSelected = value;
            GetSim().ToggleSelected(this); // not sure about this line...
        }
    }

    public bool HasStateUpdate()
    {
        bool temp = hasStateUpdate;
        hasStateUpdate = false; // reset state state once it has been accessed by its respective humble object
        return temp;
    }

    public bool HasSelectedUpdate()
    {
        bool temp = hasSelectedUpdate;
        hasSelectedUpdate = false; // reset selected state once it has been accessed by its respective humble object
        return temp;
    }

    public override string ToString()
    {
        return "Type: " + this.GetType() + "; Part Id: " + id;
    }

    public SimulationManager GetSim()
    {
        return SimulationManager.sim();
    }
}