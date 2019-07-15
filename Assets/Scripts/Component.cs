using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Component : MonoBehaviour {

    private int id;

    public void SetId (int id) {
        this.id = id;
    }

    public int GetId () {
        return id;
    }

}