﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Circuit {

    private PartWrapper[, ] partsGrid;
    private IDictionary<int, List<Part>> parts;
    private int nextId;

    public Circuit (int gridX, int gridY) {
        parts = new Dictionary<int, List<Part>> ();
        partsGrid = new PartWrapper[gridX, gridY];
        for (int i = 0; i < partsGrid.GetLength (0); i++) {
            for (int j = 0; j < partsGrid.GetLength (1); j++) {
                partsGrid[i, j] = new PartWrapper ();
            }
        }
        nextId = 0;
    }

    public int GetNextId () {
        return nextId;
    }

    public void IncrementNextId () {
        nextId++;
    }

    public void RebuildIds () { // full rebuild
        // assume that parts dictionary (which contains ids and lists of parts) is valid
        List<Part> allParts = parts.SelectMany (x => x.Value).ToList ();
        allParts.ForEach(p => p.SetId(-1)); // clearing ids

        // foreach ()
    }

    public void RecalculateIds () {
        Debug.Log ("Recalculating!");
        parts = new Dictionary<int, List<Part>> ();
        nextId = 0;
        for (int i = 0; i < partsGrid.GetLength (0); i++) { // clearing ids
            for (int j = 0; j < partsGrid.GetLength (1); j++) {
                for (int k = 0; k < 2; k++) {
                    if (partsGrid[i, j].GetParts () [k] != null) {
                        partsGrid[i, j].GetParts () [k].SetId (-1);
                    }
                }
            }
        }

        for (int i = 0; i < partsGrid.GetLength (0); i++) {
            for (int j = 0; j < partsGrid.GetLength (1); j++) { // iterating through 2d grid
                Wire left = GetLeft (i, j);
                Wire bottom = GetBottom (i, j);

                int nodeId = -2; // the id at the node (i, j)
                if (left == null && bottom == null) {
                    nodeId = nextId;
                } else if (left != null && bottom != null) {
                    nodeId = left.GetId ();
                    List<Part> bottomPathParts = GetAllOfId (bottom.GetId ()); // getting bottom path
                    for (int k = 0; k < bottomPathParts.Count; k++) { // iterating through bottom path parts
                        Part p = bottomPathParts[k];

                        // setting bottom path ids to left path ids
                        if (UpdatePartIdInDict (p, nodeId)) {
                            // return is true, a part was moved and not created
                            k--;
                        }
                    }
                } else if (left == null) { // left null, bottom not null
                    nodeId = bottom.GetId ();
                } else { // left not null, bottom null
                    nodeId = left.GetId ();
                }

                Part[] nodeParts = partsGrid[i, j].GetParts ();

                foreach (Part p in nodeParts) {
                    if (p != null) {
                        UpdatePartIdInDict (p, nodeId);
                    }
                }

                Wire top = partsGrid[i, j].GetTop ();
                Wire right = partsGrid[i, j].GetRight ();

                if (top != null || right != null) {
                    nextId++;
                }
            }
        }

        TrimIds ();

        int[] ids = new int[parts.Keys.Count];
        parts.Keys.CopyTo (ids, 0);
        foreach (int currId in ids) { // recalculating states for all ids
            CalculateStateId (currId);
        }
    }

    public void TrimIds () {
        nextId = 0;
        int[] ids = new int[parts.Keys.Count];
        parts.Keys.CopyTo (ids, 0);
        Debug.Log ("trimmin'");
        foreach (int currId in ids) {
            if (parts[currId].Count == 0) {
                parts.Remove (currId);
            } else {
                ReplaceId (currId, nextId++);
            }
        }
    }

    public void CalculateStateId (int id) {
        bool state = false;
        foreach (Part p in parts[id]) {
            if (!p.IsActive ()) // end of active parts
                break;
            state |= p.GetState ();
        }
        SetAllOfId (id, state);
    }

    public void ReplaceId (int oldId, int newId) {
        parts[newId] = parts[oldId];
        if (oldId != newId) {
            parts.Remove (oldId);
        }
        parts[newId].ForEach(p => p.SetId(newId));
    }

    public List<Part> GetAllOfId (int id) {
        return parts[id];
    }

    // soft set all, ignores active components
    public void SetAllOfId (int id, bool state) {
        GetAllOfId (id)
            .Where(p => !p.IsActive())
            .ToList()
            .ForEach(p => p.SetState(state));
    }

    // hard set all, sets ALL components
    public void SetAllOfId (int id, bool state, bool hardSet) {
        if (hardSet) { // hard set
            GetAllOfId (id).ForEach(p => p.SetState(state));
        } else { // soft set
            SetAllOfId (id, state);
        }
    }

    public void AddNode (GameObject nodeObj, Vector2Int coords) {
        GameObject tempObj = MonoBehaviour.Instantiate (nodeObj, ToVector3 (coords), Quaternion.identity);
        Part nodePart = tempObj.GetComponent<Part> ();
        nodePart.SetCoords (coords);
        nodePart.SetId (nextId);
        AddPart (nodePart);
        nextId++;
        RecalculateIds ();
    }

    public void AddWires (List<Wire> wires) {
        foreach (Wire w in wires) {
            w.SetId (nextId);
            AddPart (w);
        }

        if (wires.Count > 0) {
            nextId++;
            RecalculateIds ();
        }
    }

    private void AddPart (Part p) {
        if (p == null) return;
        Vector2Int coords = p.GetCoords ();
        if (p is Wire) {
            Wire w = (Wire) p;
            if (w.IsVertical ()) {
                partsGrid[coords.x, coords.y].SetTop (w);
            } else {
                partsGrid[coords.x, coords.y].SetRight (w);
            }
        } else {
            partsGrid[coords.x, coords.y].SetNode (p);
            Debug.Log ("Added " + p.GetType ().ToString ());
        }

        /*
        if (p is LED) {
            LED led = (LED) p;
            partsGrid[coords.x, coords.y].SetNode (led);
            Debug.Log ("ADDED AN LED!");
        } else if (p is Switch) {
            Switch sw = (Switch) p;
            partsGrid[coords.x, coords.y].SetNode (sw);
            Debug.Log ("ADDED A SWITCH!");
        } else if (p is Button) {
            Button b = (Button) p;
            partsGrid[coords.x, coords.y].SetNode (b);
            Debug.Log ("ADDED A BUTTON!");
        }*/
        AddPartToDict (p);
    }

    private void AddPartToDict (Part part) {
        int partId = part.GetId ();
        if (partId == -2 || partId == -1) {
            Debug.Log ("POTATS! partId is: " + partId);
        }
        List<Part> partsOfId = new List<Part> ();
        if (parts.ContainsKey (partId)) { // key (id) exists
            partsOfId = parts[partId];
        } else { // key (id) doesn't exist, initialize list
            parts.Add (partId, partsOfId);
        }
        if (part.IsActive ()) {
            partsOfId.Insert (0, part); // inserting active part at the front
        } else {
            partsOfId.Add (part); // adding passive part to the end
        }
    }

    private bool UpdatePartIdInDict (Part part, int newId) {
        bool idExists = parts.ContainsKey (part.GetId ());
        if (idExists) {
            List<Part> partsOfId = parts[part.GetId ()];
            partsOfId.Remove (part);
        }
        part.SetId (newId);
        AddPartToDict (part);
        return idExists && part.GetId () != newId; // prevent infinite loops
    }

    private static Vector3 ToVector3 (Vector2Int vec) {
        return new Vector3 (vec.x, vec.y, 0);
    }

    private Wire GetLeft (int x, int y) {
        if (x == 0) return null;
        return partsGrid[x - 1, y].GetRight ();
    }

    private Wire GetBottom (int x, int y) {
        if (y == 0) return null;
        return partsGrid[x, y - 1].GetTop ();
    }

}