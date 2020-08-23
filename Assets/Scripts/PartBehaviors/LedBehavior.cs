using System;
using LogicGateSimulator.Parts;

namespace LogicGateSimulator.PartBehaviors
{
    public class LedBehavior : PartBehavior
    {
        public override Type PartType => typeof(Led);

        private void Awake()
        {
            UpdateColor();
        }
    }
}
