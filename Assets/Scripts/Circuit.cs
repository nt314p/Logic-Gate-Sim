using System.Collections;
using System.Collections.Generic;
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

    public void Recalculate () {
        parts = new Dictionary<int, List<Part>> ();
        nextId = 0;
        for (int i = 0; i < partsGrid.GetLength (0); i++) { // clearing ids
            for (int j = 0; j < partsGrid.GetLength (1); j++) {
                for (int k = 0; k < 2; k++) {
                    if (partsGrid[i, j].GetWires () [k] != null) {
                        partsGrid[i, j].GetWires () [k].SetId (-1);
                    }
                }
            }
        }

        for (int i = 0; i < partsGrid.GetLength (0); i++) {
            for (int j = 0; j < partsGrid.GetLength (1); j++) {
                Wire left = GetLeft (i, j);
                Wire bottom = GetBottom (i, j);

                int nodeId = -2; // the id at the node (i, j)
                if (left == null && bottom == null) {
                    nodeId = nextId;
                } else if (left != null && bottom != null) {
                    nodeId = left.GetId ();
                    List<Part> bottomPathParts = GetAllOfId (bottom.GetId ()); // getting bottom path
                    foreach (Part p in bottomPathParts) { // iterating through bottom path wires
                        if (p is Wire) {
                            // int prevId = p.GetId();
                            // p.SetId (nodeId); 
                            UpdatePartIdInDict(p, nodeId); // setting bottom path ids to left path ids
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
                        UpdatePartIdInDict(p, nodeId);
                    }
                }

                Wire top = partsGrid[i, j].GetTop ();
                Wire right = partsGrid[i, j].GetRight ();

                if (top != null || right != null) {
                    nextId++;
                }
            }
        }

        // adding parts to dictionary
        for (int i = 0; i < partsGrid.GetLength (0); i++) { // clearing ids
            for (int j = 0; j < partsGrid.GetLength (1); j++) {
                for (int k = 0; k < 2; k++) {
                    if (partsGrid[i, j].GetWires () [k] != null) {
                        AddPartToDict(partsGrid[i, j].GetWires () [k]);
                    }
                }
            }
        }
    }

    public List<Part> GetAllOfId (int id) {
        return parts[id];
        // List<Part> ret = new List<Part> ();
        // for (int i = 0; i < partsGrid.GetLength (0); i++) {
        //     for (int j = 0; j < partsGrid.GetLength (1); j++) {
        //         Wire top = partsGrid[i, j].GetTop ();
        //         Wire right = partsGrid[i, j].GetRight ();
        //         if (top != null && top.GetId () == id) {
        //             ret.Add (top);
        //         }
        //         if (right != null && right.GetId () == id) {
        //             ret.Add (right);
        //         }
        //     }
        // }
        // return ret;
    }

    public void SetAllOfId (int id, bool state) {
        List<Part> temp = GetAllOfId (id);
        foreach (Part p in temp) {
            p.SetState (state);
        }
    }

    public void AddWires (List<Wire> wires) {
        foreach (Wire w in wires) {
            w.SetId (nextId);
            AddPart (w);
        }

        if (wires.Count > 0) {
            nextId++;
            Recalculate ();
        }
    }

    public void AddLED (Part led) {
        AddPart (led);
    }

    private void AddPart (Part p) {
        Vector2Int coords = p.GetCoords ();
        if (p is Wire) {
            Wire w = (Wire) p;
            if (w == null) return;
            if (w.IsVertical ()) {
                partsGrid[coords.x, coords.y].SetTop (w);
            } else {
                partsGrid[coords.x, coords.y].SetRight (w);
            }
        } else if (p is LED) {
            LED led = (LED) p;
            partsGrid[coords.x, coords.y].SetNode (led);
        }
        AddPartToDict(p);
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
        partsOfId.Add (part);
        Debug.Log ("Added part: " + part.ToString ());
    }

    // finds the part, removes 
    private void UpdatePartIdInDict (Part part, int newId) {
        List<Part> partsOfId = parts[part.GetId()];
        partsOfId.Remove(part);
        part.SetId(newId);
        AddPartToDict(part);
    }

    private Wire GetLeft (int x, int y) {
        if (x == 0) {
            return null;
        }
        return partsGrid[x - 1, y].GetRight ();
    }

    private Wire GetBottom (int x, int y) {
        if (y == 0) {
            return null;
        }
        return partsGrid[x, y - 1].GetTop ();
    }

}