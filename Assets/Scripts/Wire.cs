using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour {

    private Vector2Int startPoint;
    private int length;
    private bool vertical;
    private SpriteRenderer sr;
    public void Initialize (Vector2Int start, int length, bool vertical) {
        startPoint = start;
        this.length = length;
        this.vertical = vertical;
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
        if (vertical) {
            this.transform.localScale = new Vector3 (1, 10, 1);
            this.transform.position = new Vector3 (startPoint.x, startPoint.y + 0.5f, -0.01f);
        }
        if (!vertical) {
            this.transform.localScale = new Vector3 (10, 1, 1);
            this.transform.position = new Vector3 (startPoint.x + 0.5f, startPoint.y, -0.01f);
        }
    }

    public void Initialize (Vector2Int start, Vector2Int end) {
        int len = 0;
        bool vert = false;
        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x)) { 
            Vector2Int tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }
        if (start.x == end.x) {
            vert = true;
        }
        if (vertical) {
            len = Mathf.Abs (start.y - end.y);
        } else {
            len = Mathf.Abs (start.x - end.x);
        }
        Initialize(start, len, vert);
    }

}