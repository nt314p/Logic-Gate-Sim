using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour {

    private Vector2Int startPoint;
    private Vector2Int endPoint;
    private SpriteRenderer sr;

    void Start () {
        sr = this.gameObject.GetComponent<SpriteRenderer> ();
    }

    void OnMouseDown () {
        sr.color = Color.white;
        sr.sortingOrder = 1;
    }

    void OnMouseUp () {
        sr.color = new Color (0.7f, 0.7f, 0.7f);
        sr.sortingOrder = 0;
    }

    public void Initialize (Vector2Int start, Vector2Int end) {
        bool vertical = false;
        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x)) {
            Vector2Int tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }

        startPoint = start;
        endPoint = end;

        if (start.x == end.x) {
            vertical = true;
        }

        if (vertical) {
            this.transform.localScale = new Vector3 (1, 10, 1);
            this.transform.position = new Vector3 (startPoint.x, startPoint.y + 0.5f, -0.01f);
        }
        if (!vertical) {
            this.transform.localScale = new Vector3 (10, 1, 1);
            this.transform.position = new Vector3 (startPoint.x + 0.5f, startPoint.y, -0.01f);
        }
    }

}