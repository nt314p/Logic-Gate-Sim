using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {

    private static SimulationManager instance = null;
    public static GameObject wire;
    public static GameObject LED;
    public static GameObject Switch;
    public static GameObject Button;
    private List<GameObject> wiresInPath;
    private List<Vector2Int> wirePath;
    public bool drawingWirePath;
    private string selectedPart;
    private Circuit currentCircuit;
    private List<Part> selectedParts;

    public Dictionary<KeyCode, string> keybinds = new Dictionary<KeyCode, string> { { KeyCode.W, "wire" },
        { KeyCode.L, "led" },
        { KeyCode.S, "switch" },
        { KeyCode.B, "button" }
    };

    // Start is called before the first frame update
    void Start () {
        Application.targetFrameRate = 60;
        instance = this;
        wire = (GameObject) Resources.Load ("Prefabs/Wire", typeof (GameObject));
        LED = (GameObject) Resources.Load ("Prefabs/LED", typeof (GameObject));
        Switch = (GameObject) Resources.Load ("Prefabs/Switch", typeof (GameObject));
        Button = (GameObject) Resources.Load ("Prefabs/Button", typeof (GameObject));
        Debug.Log (Button);

        wirePath = new List<Vector2Int> ();
        wiresInPath = new List<GameObject> ();
        drawingWirePath = false;
        selectedPart = "";
        selectedParts = new List<Part> ();

        currentCircuit = new Circuit (GridController.width, GridController.height);
        currentCircuit.RecalculateIds ();
    }

    // Update is called once per frame
    void Update () {
        bool pressed = false;
        foreach (KeyValuePair<KeyCode, string> entry in keybinds) {
            if (Input.GetKey (entry.Key)) {
                selectedPart = entry.Value;
                pressed = true;
            }
        }
        if (!pressed) {
            selectedPart = "";
        }

        // Debug.Log(selectedPart);

        Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2Int coord = new Vector2Int (Mathf.RoundToInt (temp.x), Mathf.RoundToInt (temp.y));

        if (Input.GetMouseButtonDown (0)) {
            switch (selectedPart) {
                case "wire":
                    if (IsWithinBounds (coord)) {
                        wirePath.Add (coord);
                        drawingWirePath = true;
                    }
                    break;
                case "led":
                    currentCircuit.AddNode (LED, coord);
                    break;
                case "switch":
                    currentCircuit.AddNode (Switch, coord);
                    break;
                case "button":
                    currentCircuit.AddNode (Button, coord);
                    break;
                default:
                    break;
            }

        } else if (Input.GetMouseButton (0)) {
            if (drawingWirePath) {
                Vector2Int prevCoord = wirePath[wirePath.Count - 1];

                // coordinate is unique and in grid bounds
                if (!(coord.Equals (prevCoord)) && IsWithinBounds (coord)) {
                    // is the wire going back on itself
                    if (wirePath.Count >= 2 && wirePath[wirePath.Count - 2] == coord) {
                        Destroy (wiresInPath[wiresInPath.Count - 1]); // removing wire
                        wirePath.RemoveAt (wirePath.Count - 1);
                        wiresInPath.RemoveAt (wiresInPath.Count - 1);
                    } else { // adding new wire
                        List<Vector2Int> interpolated = Interpolate (prevCoord, coord);
                        for (int i = 0; i < interpolated.Count; i++) {
                            wirePath.Add (interpolated[i]);
                            Vector2Int start = wirePath[wirePath.Count - 2];
                            Vector2Int end = wirePath[wirePath.Count - 1];
                            wiresInPath.Add (Instantiate (wire, ToVector3 (start), Quaternion.identity));
                            wiresInPath[wiresInPath.Count - 1].GetComponent<Wire> ().Initialize (start, end);
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            List<Wire> wireList = new List<Wire> ();
            for (int i = 0; i < wiresInPath.Count; i++) {
                wireList.Add (wiresInPath[i].GetComponent<Wire> ());
            }
            if (wireList.Count > 0)
                currentCircuit.AddWires (wireList);
            wiresInPath = new List<GameObject> (); // resetting variables
            wirePath = new List<Vector2Int> ();
            drawingWirePath = false;
        }
    }

    public string GetSelectedPart () {
        return selectedPart;
    }

    public Circuit GetCircuit () {
        return currentCircuit;
    }

    public bool ToggleSelected (Part p) {
        if (selectedParts.Contains (p)) {
            selectedParts.Remove (p);
        } else {
            selectedParts.Add (p);
        }
        return !selectedParts.Contains (p);
    }

    public void ClearSelected () {
        while (selectedParts.Count != 0) {
            selectedParts[0].SetSelected (false);
        }
    }

    private static Vector3 ToVector3 (Vector2Int vec) {
        return new Vector3 (vec.x, vec.y, 0);
    }

    // returns the angle of the vector formed by the vector2s passed in
    private float AngleOf (Vector2Int start, Vector2Int end) {
        float angle = Mathf.Atan2 ((start.y - end.y), (start.x - end.x)) * Mathf.Rad2Deg;
        if (angle < 0) angle += 180; // we want a positive angle
        return angle % 180;
    }

    // Interpolate method determines the horizontal and vertical steps to model a diagonal line
    private List<Vector2Int> Interpolate (Vector2Int start, Vector2Int end) {
        List<Vector2Int> ret = new List<Vector2Int> (); // list of the coordinates of the steps
        ret.Add (start);

        float targetAngle = AngleOf (start, end); // determining angle of the diagonal line to approximate

        int steps = Mathf.Abs (start.x - end.x) + Mathf.Abs (start.y - end.y); // compute total steps
        int signX = Mathf.RoundToInt (Mathf.Sign ((end.x - start.x))); // which direction steps
        int signY = Mathf.RoundToInt (Mathf.Sign ((end.y - start.y))); // go in (up down left right)

        for (int i = 0; i < steps; i++) { // stepping and incrementing
            Vector2Int last = ret[ret.Count - 1];
            Vector2Int vStep = new Vector2Int (last.x, last.y + signY); // computing both possible steps
            Vector2Int hStep = new Vector2Int (last.x + signX, last.y);

            // the angle formed when the last coordinate is incremented by a vertical step
            float vAngle = AngleOf (start, vStep);
            // the angle formed when the last coordinate is incremented by a horizontal step
            float hAngle = AngleOf (start, hStep);

            if (Mathf.Abs (vAngle - targetAngle) < Mathf.Abs (hAngle - targetAngle)) {
                ret.Add (vStep); // vertical step brings us closer to the target angle
            } else {
                ret.Add (hStep); // horizontal step brings us closer to the target angle
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

    public static SimulationManager sim () {
        return instance;
    }

}