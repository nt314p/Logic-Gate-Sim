using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour {

    public GameObject wire;
    private List<GameObject> wires;
    private List<GameObject> wiresInPath;
    private List<Vector2Int> wirePath;

    // Start is called before the first frame update
    void Start () {
        wirePath = new List<Vector2Int> ();
        wiresInPath = new List<GameObject> ();
        wires = new List<GameObject> ();

        interpolate (new Vector2Int (0, 0), new Vector2Int (1, 1));
    }

    // Update is called once per frame
    void Update () {

        Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2Int coord = new Vector2Int (Mathf.RoundToInt (temp.x), Mathf.RoundToInt (temp.y));

        if (Input.GetMouseButtonDown (0)) {
            wirePath.Add (coord);
        } else if (Input.GetMouseButton (0)) {
            Vector2Int prevCoord = wirePath[wirePath.Count - 1];
            if (!(coord.Equals (prevCoord))) {
                if (wirePath.Count >= 2 && wirePath[wirePath.Count - 2] == coord) {
                    Destroy (wiresInPath[wiresInPath.Count - 1]);
                    wirePath.RemoveAt (wirePath.Count - 1);
                    wiresInPath.RemoveAt (wiresInPath.Count - 1);
                } else {
                    List<Vector2Int> interpolated = interpolate (prevCoord, coord);
                    for (int i = 0; i < interpolated.Count; i++) {
                        wirePath.Add (interpolated[i]);
                        Vector2Int start = wirePath[wirePath.Count - 2];
                        Vector2Int end = wirePath[wirePath.Count - 1];
                        wiresInPath.Add (Instantiate (wire, toVector3 (start), Quaternion.identity));
                        wiresInPath[wiresInPath.Count - 1].GetComponent<Wire> ().Initialize (start, end);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            for (int i = 0; i < wiresInPath.Count; i++) {
                wires.Add (wiresInPath[i]);
            }
            wiresInPath = new List<GameObject> ();
            wirePath = new List<Vector2Int> ();
        }
    }

    private Vector3 toVector3 (Vector2Int vec) {
        return new Vector3 (vec.x, vec.y, 0);
    }

    // returns the angle of the vector formed by the vector2s passed in
    private float angleOf (Vector2Int start, Vector2Int end) { 
        float angle = Mathf.Atan2 ((start.y - end.y), (start.x - end.x)) * Mathf.Rad2Deg;
        if (angle < 0) angle += 180;
        return angle % 180;
    }

    private List<Vector2Int> interpolate (Vector2Int start, Vector2Int end) {
        List<Vector2Int> ret = new List<Vector2Int> ();
        ret.Add (start);

        float targetAngle = angleOf(start, end);

        int steps = Mathf.Abs (start.x - end.x) + Mathf.Abs (start.y - end.y);
        int signX = Mathf.RoundToInt (Mathf.Sign ((end.x - start.x)));
        int signY = Mathf.RoundToInt (Mathf.Sign ((end.y - start.y)));

        for (int i = 0; i < steps; i++) { // stepping and incrementing
            Vector2Int last = ret[ret.Count - 1];
            Vector2Int vStep = new Vector2Int (last.x, last.y + signY);
            Vector2Int hStep = new Vector2Int (last.x + signX, last.y);

            // the angle formed when the last coordinate is incremented by a vertical step
            float vAngle = angleOf (start, vStep);
            // the angle formed when the last coordinate is incremented by a horizontal step
            float hAngle = angleOf (start, hStep);

            if (Mathf.Abs(vAngle - targetAngle) < Mathf.Abs(hAngle - targetAngle)) {
                ret.Add (vStep); // vertical step brings us closer to the target angle
            } else {
                ret.Add (hStep);
            }
        }
        ret.RemoveAt (0); // removing starting coordinate
        return ret;
    }
}