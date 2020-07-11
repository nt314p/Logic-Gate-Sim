﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Wire : Part {

    private Vector2Int endPoint;
    private Vector2Int orientation; // either V2.up or V2.right
    private SpriteRenderer sr;
    private readonly float len = 1.38f;

    void Awake () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        this.State = false;
        this.Active = false;
        UpdateColor ();
    }

    void OnMouseDown () {
        this.Selected = !this.Selected;
        UpdateColor ();
    }

    void OnMouseUp () {
        UpdateColor ();
        sr.sortingOrder = 0;
    }

    public void Initialize (Vector2Int start, Vector2Int end, int id) {
        Id = id;

        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x)) {
            Vector2Int tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }

        this.Coords = start;
        endPoint = end;

        orientation = end - start;
        Vector3 direction = (Vector2) orientation;

        this.transform.localScale = new Vector3 (len, len, 1) + direction * 9 * len;
        this.transform.position = new Vector3 (this.Coords.x, this.Coords.y, -0.01f) + direction * 0.5f;
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
        if (Selected) {
            sr.color = this.SelectedColor;
            sr.sortingOrder = 1;
        } else {
            sr.color = this.State ? this.ActiveColor : this.InactiveColor;
        }
    }

    public Vector2Int GetEndPoint () {
        return endPoint;
    }

    public Vector2Int GetOrientation () {
        return orientation;
    }

    public bool Equals (Wire w) {
        return this.Coords.Equals (w.Coords) && endPoint.Equals (w.endPoint);
    }
}