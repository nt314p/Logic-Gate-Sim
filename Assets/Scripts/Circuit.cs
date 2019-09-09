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
            for (int j = 0; j < partsGrid.GetLength (1); j++) {
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
        Debug.Log ("Recalculation Complete!");
    }

    public List<Part> GetAllOfId (int id) {
        return parts[id];
    }

    public void SetAllOfId (int id, bool state) {
        List<Part> temp = GetAllOfId (id);
        foreach (Part p in temp) {
            p.SetState (state);
        }
    }

    public void AddNode(GameObject nodeObj, Vector2Int coords) {
        GameObject tempObj = MonoBehaviour.Instantiate (nodeObj, ToVector3 (coords), Quaternion.identity);
        Part nodePart = tempObj.GetComponent<Part>();
        nodePart.SetCoords (coords);
        nodePart.SetId (nextId);
        AddPart(nodePart);
        nextId++;
        Recalculate();
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
            Debug.Log ("ADDED AN LED!");
        } else if (p is Switch) {
            Switch sw = (Switch) p;
            partsGrid[coords.x, coords.y].SetNode (sw);
            Debug.Log ("ADDED A SWITCH!");
        }
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
        partsOfId.Add (part);
    }

    private bool UpdatePartIdInDict (Part part, int newId) {
        bool idExists = parts.ContainsKey (part.GetId ());
        bool hasSameId = part.GetId () == newId;
        if (idExists) {
            List<Part> partsOfId = parts[part.GetId ()];
            partsOfId.Remove (part);
        }
        part.SetId (newId);
        AddPartToDict (part);
        return idExists && !hasSameId; // prevent infinite loops
    }

    private static Vector3 ToVector3 (Vector2Int vec) {
        return new Vector3 (vec.x, vec.y, 0);
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