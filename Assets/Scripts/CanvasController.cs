using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
    // Start is called before the first frame update

    private Text coordText;
    private GameObject cam;

    void Start () {
        cam = Camera.main.gameObject;
        coordText = transform.Find("CoordinateText").gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2Int coord = new Vector2Int (Mathf.RoundToInt (temp.x), Mathf.RoundToInt (temp.y));
        coordText.text = coord.ToString ();
    }
}