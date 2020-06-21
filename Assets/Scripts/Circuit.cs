using System.Collections;
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
        allParts.ForEach (p => p.Id = -1); // clearing ids

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
                        partsGrid[i, j].GetParts () [k].Id = -1;
                    }
                }
            }
        }

        for (int i = 0; i < partsGrid.GetLength (0); i++) {
            for (int j = 0; j < partsGrid.GetLength (1); j++) { // iterating through 2d grid
                Wire left = GetWire (new Vector2Int (i, j), Vector2Int.left);
                Wire bottom = GetWire (new Vector2Int (i, j), Vector2Int.down);

                int nodeId = -2; // the id at the node (i, j)
                if (left == null && bottom == null) {
                    nodeId = nextId;
                } else if (left != null && bottom != null) {
                    nodeId = left.Id;
                    List<Part> bottomPathParts = GetAllOfId (bottom.Id); // getting bottom path
                    for (int k = 0; k < bottomPathParts.Count; k++) { // iterating through bottom path parts
                        Part p = bottomPathParts[k];

                        // setting bottom path ids to left path ids
                        if (UpdatePartIdInDict (p, nodeId)) {
                            // return is true, a part was moved and not created
                            k--;
                        }
                    }
                } else if (left == null) { // left null, bottom not null
                    nodeId = bottom.Id;
                } else { // left not null, bottom null
                    nodeId = left.Id;
                }

                Part[] nodeParts = partsGrid[i, j].GetParts ();

                foreach (Part p in nodeParts) {
                    if (p != null) {
                        UpdatePartIdInDict (p, nodeId);
                    }
                }

                Wire top = partsGrid[i, j].GetWire (Vector2Int.up);
                Wire right = partsGrid[i, j].GetWire (Vector2Int.right);

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

    // calculates the state of an id based on the states of its active components
    public void CalculateStateId (int id) {
        bool state = false;
        foreach (Part p in parts[id]) {
            if (!p.IsActive () || state) break; // end of active parts or state = true -> can't be made false
            state |= p.State;
        }
        SetAllOfId (id, state);
    }

    public void ReplaceId (int oldId, int newId) {
        if (oldId == newId) return;
        parts[newId] = parts[oldId]; // set list reference in dict to new id
        parts.Remove (oldId);
        parts[newId].ForEach (p => p.Id = newId);
    }

    // links all part ids passed in
    public void LinkIds (Part[] parts) {
        int[] validPartIds = parts.Where (p => p != null).Select (p => p.Id).ToArray ();
        for (int i = 1; i < validPartIds.Length; ++i) {
            ReplaceId (validPartIds[i], validPartIds[0]); // link other ids to first id
        }
    }

    public Wire[] GetWiresFromDirection (Vector2Int coords, Vector2Int direction, bool ignoreConnected) {
        float angle = Mathf.Atan2 (direction.y, direction.x);
        return null;
    }

    public List<Part> GetAllOfId (int id) {
        return parts[id];
    }

    // hard set all, sets ALL components, soft set will ignore active components
    public void SetAllOfId (int id, bool state, bool hardSet = false) {
        List<Part> editParts = GetAllOfId (id);
        if (!hardSet) { // soft set filters out active parts
            editParts = editParts.Where (p => !p.IsActive ()).ToList ();
        }
        editParts.ForEach (p => p.State = state);
    }

    public void AddNode (GameObject nodeObj, Vector2Int coords) {
        GameObject tempObj = MonoBehaviour.Instantiate (nodeObj, ToVector3 (coords), Quaternion.identity);
        Part nodePart = tempObj.GetComponent<Part> ();
        nodePart.SetCoords (coords);
        nodePart.Id = nextId;
        AddPart (nodePart);
        nextId++;
        RecalculateIds ();
    }

    public void AddWires (List<Wire> wires) {
        foreach (Wire w in wires) {
            w.Id = nextId;
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
            partsGrid[coords.x, coords.y].SetWire (w, w.GetOrientation ());
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
        int partId = part.Id;
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
        bool idExists = parts.ContainsKey (part.Id);
        if (idExists) {
            List<Part> partsOfId = parts[part.Id];
            partsOfId.Remove (part);
        }
        part.Id = newId;
        AddPartToDict (part);
        return idExists && part.Id != newId; // prevent infinite loops
    }

    private static Vector3 ToVector3 (Vector2Int vecInt) {
        return (Vector3) (Vector2) vecInt;
    }

    // gets a wire based on direction from the part grid
    private Wire GetWire (Vector2Int coord, Vector2Int direction) {
        if (coord.x == 0 || coord.y == 0) return null;
        if (direction.x == -1 || direction.y == -1) { // left or bottom
            Vector2Int shifted = coord + direction;
            return partsGrid[shifted.x, shifted.y].GetWire (-direction);
        } else { // top or right
            return partsGrid[coord.x, coord.y].GetWire (direction);
        }
    }
}