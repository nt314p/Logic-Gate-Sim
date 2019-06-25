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
                    wirePath.Add (coord);
                    Vector2Int start = wirePath[wirePath.Count - 2];
                    Vector2Int end = wirePath[wirePath.Count - 1];
                    wiresInPath.Add (Instantiate (wire, toVector3 (start), Quaternion.identity));
                    wiresInPath[wiresInPath.Count - 1].GetComponent<Wire> ().Initialize (start, end);
                }
            }
        }

        // for (int i = 0; i < wirePath.Count - 1; i++) {
        //     Vector2Int start = wirePath[i];
        //     Vector2Int end = wirePath[i + 1];
        //     wiresInPath.Add (Instantiate (wire, toVector3 (start), Quaternion.identity));
        //     wiresInPath[wiresInPath.Count - 1].GetComponent<Wire> ().Initialize (start, end);
        // }

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
}