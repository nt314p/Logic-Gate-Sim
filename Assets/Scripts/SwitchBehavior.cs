using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBehavior : PartBehavior
{
    private Transform toggle;
    private float currY;
    private float targetY;
    private readonly float offset = 0.015f;
    private readonly float targetTolerance = 0.002f;
    private readonly float toggleSpeed = 0.14f;

    void Awake()
    {
        targetY = -offset;
        currY = targetY;
        toggle = transform.Find("Toggle");
    }

    public override void ChildUpdate()
    {
        toggle.localPosition = Vector3.up * currY; // new Vector3(0, currY, 0); 

        if (Mathf.Abs(currY - targetY) < targetTolerance)
        {
            currY = Mathf.Sign(currY) * offset;
        }
        else
        {
            currY += toggleSpeed * Time.deltaTime * -Mathf.Sign(currY - targetY);
        }
    }

    public override void UpdateColor()
    {
        
    }

    public override void OnStateUpdate()
    {
        targetY = offset * (PartObj.State ? 1 : -1);
    }
}
