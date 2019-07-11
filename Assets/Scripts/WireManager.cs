using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour {

    public GameObject wire;
    private WireWrapper[, ] masterGrid;
    private int nextId;
    private List<GameObject> wiresInPath;
    private List<Vector2Int> wirePath;

    // Start is called before the first frame update
    void Start () {
        wirePath = new List<Vector2Int> ();
        wiresInPath = new List<GameObject> ();
        masterGrid = new WireWrapper[50, 50];
        for (int i = 0; i < masterGrid.GetLength (0); i++) {
            for (int j = 0; j < masterGrid.GetLength (1); j++) {
                masterGrid[i, j] = new WireWrapper ();
            }
        }
        nextId = 0;

        Recalculate ();
    }

    // Update is called once per frame
    void Update () {

        Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2Int coord = new Vector2Int (Mathf.RoundToInt (temp.x), Mathf.RoundToInt (temp.y));

        if (Input.GetMouseButtonDown (0)) {
            if (IsWithinBounds (coord)) {
                wirePath.Add (coord);
            }
        } else if (Input.GetMouseButton (0)) {
            Vector2Int prevCoord = wirePath[wirePath.Count - 1];

            // coordinate is unique and in grid bounds
            if (!(coord.Equals (prevCoord)) && IsWithinBounds (coord)) {
                if (wirePath.Count >= 2 && wirePath[wirePath.Count - 2] == coord) {
                    Destroy (wiresInPath[wiresInPath.Count - 1]);
                    wirePath.RemoveAt (wirePath.Count - 1);
                    wiresInPath.RemoveAt (wiresInPath.Count - 1);
                } else {
                    List<Vector2Int> interpolated = Interpolate (prevCoord, coord);
                    for (int i = 0; i < interpolated.Count; i++) {
                        wirePath.Add (interpolated[i]);
                        Vector2Int start = wirePath[wirePath.Count - 2];
                        Vector2Int end = wirePath[wirePath.Count - 1];
                        wiresInPath.Add (Instantiate (wire, ToVector3 (start), Quaternion.identity));
                        wiresInPath[wiresInPath.Count - 1].GetComponent<Wire> ().Initialize (start, end, nextId);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            for (int i = 0; i < wiresInPath.Count; i++) {
                AddToMasterGrid (wiresInPath[i].GetComponent<Wire> ());
            }
            if (wiresInPath.Count > 0) {
                nextId++;
                Recalculate ();
            }
            wiresInPath = new List<GameObject> ();
            wirePath = new List<Vector2Int> ();
        }
    }

    private void Recalculate () {
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

    private List<Wire> GetAllOfId (int id) {
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

    private Vector3 ToVector3 (Vector2Int vec) {
        return new Vector3 (vec.x, vec.y, 0);
    }

    // returns the angle of the vector formed by the vector2s passed in
    private float AngleOf (Vector2Int start, Vector2Int end) {
        float angle = Mathf.Atan2 ((start.y - end.y), (start.x - end.x)) * Mathf.Rad2Deg;
        if (angle < 0) angle += 180;
        return angle % 180;
    }

    private void AddToMasterGrid (Wire w) {
        Vector2Int coord = w.GetStartPoint ();
        if (w.IsVertical ()) {
            masterGrid[coord.x, coord.y].SetTop (w);
        } else {
            masterGrid[coord.x, coord.y].SetRight (w);
        }
    }

    private List<Vector2Int> Interpolate (Vector2Int start, Vector2Int end) {
        List<Vector2Int> ret = new List<Vector2Int> ();
        ret.Add (start);

        float targetAngle = AngleOf (start, end);

        int steps = Mathf.Abs (start.x - end.x) + Mathf.Abs (start.y - end.y);
        int signX = Mathf.RoundToInt (Mathf.Sign ((end.x - start.x)));
        int signY = Mathf.RoundToInt (Mathf.Sign ((end.y - start.y)));

        for (int i = 0; i < steps; i++) { // stepping and incrementing
            Vector2Int last = ret[ret.Count - 1];
            Vector2Int vStep = new Vector2Int (last.x, last.y + signY);
            Vector2Int hStep = new Vector2Int (last.x + signX, last.y);

            // the angle formed when the last coordinate is incremented by a vertical step
            float vAngle = AngleOf (start, vStep);
            // the angle formed when the last coordinate is incremented by a horizontal step
            float hAngle = AngleOf (start, hStep);

            if (Mathf.Abs (vAngle - targetAngle) < Mathf.Abs (hAngle - targetAngle)) {
                ret.Add (vStep); // vertical step brings us closer to the target angle
            } else {
                ret.Add (hStep);
            }
        }
        ret.RemoveAt (0); // removing starting coordinate
        return ret;
    }

    private bool IsWithinBounds (Vector2Int vec) {
        Vector2Int clamped = new Vector2Int (vec.x, vec.y);
        clamped.Clamp (Vector2Int.zero, new Vector2Int (49, 49));
        return clamped.Equals (vec);
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