using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Part {

    private Vector2Int endPoint;
    private Vector2Int orientation; // either V2.up or V2.right
    private SpriteRenderer sr;
    private readonly float len = 1.38f;

    void Awake () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        SetState (false);
        SetIsActive (false);
        UpdateColor ();
    }

    void OnMouseDown () {
        SetSelected (!IsSelected ());
        UpdateColor ();
    }

    void OnMouseUp () {
        UpdateColor ();
        sr.sortingOrder = 0;
    }

    public void Initialize (Vector2Int start, Vector2Int end, int id) {
        SetId (id);

        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x)) {
            Vector2Int tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }

        SetCoords (start);
        endPoint = end;

        orientation = end - start;
        Vector3 direction = (Vector2) orientation;

        this.transform.localScale = new Vector3 (len, len, 1) + direction * 9 * len;
        this.transform.position = new Vector3 (GetCoords ().x, GetCoords ().y, -0.01f) + direction * 0.5f;
    }

    public void Initialize (Vector2Int start, Vector2Int end) {
        Initialize (start, end, -1);
    }

    public override void OnStateUpdate () {
        UpdateColor ();
    }

    public override void OnSelectUpdate () {
        UpdateColor ();
    }

    public void UpdateColor () {
        if (IsSelected ()) {
            sr.color = Part.colorSelected;
            sr.sortingOrder = 1;
        } else if (GetState ()) {
            sr.color = Part.colorActive;
        } else {
            sr.color = Part.colorInactive;
        }
    }

    public Vector2Int GetEndPoint () {
        return endPoint;
    }

    public Vector2Int GetOrientation () {
        return orientation;
    }

    public bool Equals (Wire w) {
        return GetCoords ().Equals (w.GetCoords ()) && endPoint.Equals (w.endPoint);
    }
}