using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour {

    private Vector2Int startPoint;
    private Vector2Int endPoint;
    private bool vertical;
    private SpriteRenderer sr;
    private int id;

    void Start () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        sr.color = new Color (0f, 0.65f, 0.075f);
    }

    void OnMouseDown () {
        sr.color = new Color (0f, 0.79f, 0.09f);
        sr.sortingOrder = 1;
        Debug.Log ("ID: " + id);
    }

    void OnMouseUp () {
        sr.color = new Color (0f, 0.65f, 0.075f);
        sr.sortingOrder = 0;
    }

    public void Initialize (Vector2Int start, Vector2Int end, int id) {
        this.id = id;
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

    public Vector2Int GetStartPoint () {
        return startPoint;
    }

    public void SetId (int id) {
        this.id = id;
    }

    public int GetId () {
        return id;
    }

    public bool IsVertical () {
        return vertical;
    }

    public bool Equals (Wire w) {
        return startPoint.Equals (w.startPoint) && endPoint.Equals (w.endPoint);
    }

}