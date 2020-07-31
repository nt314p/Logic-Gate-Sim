using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDBehavior : MonoBehaviour
{
    private SpriteRenderer _sr;
    private LED _led;

    void Awake()
    {
        _sr = this.gameObject.GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateColor()
    {
        _sr.color = this.State ? this.ActiveColor : this.InactiveColor;
    }

    void OnMouseDown()
    {
        Debug.Log("clicked " + this.ToString());
        _sr.color = Part.SelectedColor;
    }

    void OnMouseUp()
    {
        UpdateColor();
    }

    public override void OnStateUpdate()
    {
        UpdateColor();
    }

    public override void OnSelectUpdate()
    {
        throw new System.NotImplementedException();
    }
}
