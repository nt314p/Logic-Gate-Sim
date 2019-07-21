using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Component {

    private Vector2Int startPoint;
    private Vector2Int endPoint;
    private bool vertical;
    private SpriteRenderer sr;

    void Start () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        SetState (false);
        UpdateColor ();
    }

    void OnMouseDown () {
        if (Input.GetKey (KeyCode.LeftControl)) {
            SetState (!GetState ());
            FindObjectOfType<WireManager> ().SetAllOfId (GetId (), GetState ());
        }
        sr.color = new Color (0.67f, 0.89f, 0f);
        sr.sortingOrder = 1;
    }

    void OnMouseUp () {
        UpdateColor ();
        sr.sortingOrder = 0;
    }

    public void Initialize (Vector2Int start, Vector2Int end, int id) {
        SetId (id);
        vertical = false;

        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x)) {
            Vector2Int tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }

        startPoint = start;
        endPoint = end;

        if (start.x == end.x) {
            vertical = true;
            this.transform.localScale = new Vector3 (1.38f, 13.8f, 1);
            this.transform.position = new Vector3 (startPoint.x, startPoint.y + 0.5f, -0.01f);
        } else {
            this.transform.localScale = new Vector3 (13.8f, 1.38f, 1);
            this.transform.position = new Vector3 (startPoint.x + 0.5f, startPoint.y, -0.01f);
        }
    }

    public void Initialize (Vector2Int start, Vector2Int end) {
        Initialize (start, end, -1);
    }

    public override void OnStateUpdate () {
        UpdateColor ();
    }

    public void UpdateColor () {
        if (GetState ()) {
            sr.color = new Color (0f, 0.79f, 0.09f);
        } else {
            sr.color = new Color (0f, 0.494f, 0.0588f);
        }
    }

    public Vector2Int GetStartPoint () {
        return startPoint;
    }

    public bool IsVertical () {
        return vertical;
    }

    public bool Equals (Wire w) {
        return startPoint.Equals (w.startPoint) && endPoint.Equals (w.endPoint);
    }

}