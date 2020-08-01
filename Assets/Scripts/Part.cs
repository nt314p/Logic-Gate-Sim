using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public abstract class Part
{
    private int id;
    private bool state;
    private bool hasStateUpdate;
    private Vector2Int coords;
    private bool isActive; // if true state cannot be changed externally

    public Part(bool isActive, int id = -1)
    {
        this.isActive = isActive;
        this.State = false;
        this.Id = id;
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

    public bool HasStateUpdate()
    {
        bool temp = hasStateUpdate;
        hasStateUpdate = false; // reset state state once it has been accessed by its respective humble object
        return temp;
    }

    public override string ToString()
    {
        return $"Type: {this.GetType()}; Id: {this.Id}";
    }
}