using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Part {

    private Vector2Int endPoint;
    private bool vertical;
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
        vertical = false;

        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x)) {
            Vector2Int tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }

        SetCoords (start);
        endPoint = end;

        Vector3 direction;
        vertical = start.x == end.x;
        direction = vertical ? Vector3.up : Vector3.right;

        this.transform.localScale = new Vector3 (len, len, 1) + direction * 9 * len;
        this.transform.position = new Vector3 (GetCoords ().x, GetCoords ().y, -0.01f) + direction * 0.5f;
        Debug.Log (this.transform.position);
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
            sr.color = new Color (0.67f, 0.89f, 0f);
            sr.sortingOrder = 1;
        } else if (GetState ()) {
            sr.color = Part.colorActive;
        } else {
            sr.color = Part.colorInactive;
        }
    }

    public bool IsVertical () {
        return vertical;
    }

    public bool Equals (Wire w) {
        return GetCoords ().Equals (w.GetCoords ()) && endPoint.Equals (w.endPoint);
    }

}