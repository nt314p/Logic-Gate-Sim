﻿using System;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.PartBehaviors
{
    public class ButtonBehavior : PartBehavior
    {
        public override Type PartType => typeof(Button);

        private void OnMouseUpAsButton()
        {
            PartObject.State = false;

            // PartObject.State = false; // untoggle button
            //GetSim().GetCircuit().CalculateStateId(PartObject.Id); // fix this sketchy code
            // perhaps add a "updated" boolean that is set if the state has been updated
            // then the Circuit iterates through active parts and sees if any parts have been updated
            // if so, then update the part (and its corresponding ids)
        }
    }
}