using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit {

    private PartWrapper[, ] masterGrid;
    private int nextId;

    public void Initialize (int gridX, int gridY) {
        masterGrid = new PartWrapper[gridX, gridY];
        for (int i = 0; i < masterGrid.GetLength (0); i++) {
            for (int j = 0; j < masterGrid.GetLength (1); j++) {
                masterGrid[i, j] = new PartWrapper ();
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
        nextId = 0;
        for (int i = 0; i < masterGrid.GetLength (0); i++) { // clearing ids
            for (int j = 0; j < masterGrid.GetLength (1); j++) {
                for (int k = 0; k < 2; k++) {
                    if (masterGrid[i, j].GetWires () [k] != null) {
                        masterGrid[i, j].GetWires () [k].SetId (-1);
                    }
                }
            }
        }

        for (int i = 0; i < masterGrid.GetLength (0); i++) {
            for (int j = 0; j < masterGrid.GetLength (1); j++) {
                Wire left = GetLeft (i, j);
                Wire bottom = GetBottom (i, j);

                int nodeId = -2; // the id at the nod (i, j)
                if (left == null && bottom == null) {
                    nodeId = nextId;
                } else if (left != null && bottom != null) {
                    nodeId = left.GetId ();
                    List<Wire> bottomPath = GetAllOfId (bottom.GetId ()); // getting bottom path
                    foreach (Wire w in bottomPath) { // iterating through bottom path wires
                        w.SetId (nodeId); // setting bottom path ids to left path ids
                    }
                } else if (left == null) { // left null, bottom not null
                    nodeId = bottom.GetId ();
                } else { // left not null, bottom null
                    nodeId = left.GetId ();
                }
                Wire top = masterGrid[i, j].GetTop ();
                Wire right = masterGrid[i, j].GetRight ();
                if (top != null) {
                    top.SetId (nodeId);
                }
                if (right != null) {
                    right.SetId (nodeId);
                }
                if (top != null || right != null) {
                    nextId++;
                }
            }
        }
    }

    public List<Wire> GetAllOfId (int id) {
        List<Wire> ret = new List<Wire> ();
        for (int i = 0; i < masterGrid.GetLength (0); i++) {
            for (int j = 0; j < masterGrid.GetLength (1); j++) {
                Wire top = masterGrid[i, j].GetTop ();
                Wire right = masterGrid[i, j].GetRight ();
                if (top != null && top.GetId () == id) {
                    ret.Add (top);
                }
                if (right != null && right.GetId () == id) {
                    ret.Add (right);
                }
            }
        }
        return ret;
    }

    public void SetAllOfId (int id, bool state) {
        List<Wire> temp = GetAllOfId (id);
        foreach (Wire w in temp) {
            w.SetState (state);
        }
    }

    public void AddWires (List<Wire> wires) {
        foreach (Wire w in wires) {
            w.SetId(nextId);
            AddToMasterGrid (w);
        }
        
        if (wires.Count > 0) {
            nextId++;
            Recalculate ();
        }
    }

    public void AddLED()

    private void AddToMasterGrid (Wire w) {
        if (w == null) return;
        Vector2Int coord = w.GetStartPoint ();
        if (w.IsVertical ()) {
            masterGrid[coord.x, coord.y].SetTop (w);
        } else {
            masterGrid[coord.x, coord.y].SetRight (w);
        }
    }

    private Wire GetLeft (int x, int y) {
        if (x == 0) {
            return null;
        }
        return masterGrid[x - 1, y].GetRight ();
    }

    private Wire GetBottom (int x, int y) {
        if (y == 0) {
            return null;
        }
        return masterGrid[x, y - 1].GetTop ();
    }

}