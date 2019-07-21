using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Component : MonoBehaviour {

    private int id;
    private bool state;

    public void SetId (int id) {
        this.id = id;
    }

    public int GetId () {
        return id;
    }

    public bool GetState () {
        return state;
    }

    public void SetState(bool state) {
        this.state = state;
        OnStateUpdate();
    }

    public abstract void OnStateUpdate();

}