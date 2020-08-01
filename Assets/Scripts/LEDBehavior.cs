using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDBehavior : PartBehavior
{
    private void Awake()
    {
        this.SRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        UpdateColor();
    }
}
