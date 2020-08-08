using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : PartBehavior
{
    void Awake()
    {

    }

    void OnMouseUpAsButton()
    {
        PartObject.State = false; // untoggle button
        GetSim().GetCircuit().CalculateStateId(PartObject.Id); // fix this sketchy code
                                                         // perhaps add a "updated" boolean that is set if the state has been updated
                                                         // then the Circuit iterates through active parts and sees if any parts have been updated
                                                         // if so, then update the part (and its corresponding ids)
    }
}
