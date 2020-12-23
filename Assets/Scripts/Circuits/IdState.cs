using System;
using System.Collections.Generic;

namespace LogicGateSimulator.Circuits
{
    public class IdState
    {
        public bool State { get; set; }
        public List<Gate> LogicGates { get; }

        public struct Gate
        {
            public int AId;
            public int BId;
            public Circuit.GateType GateType;

            public Gate(int aId, int bId, Circuit.GateType gateType)
            {
                AId = aId;
                BId = bId;
                GateType = gateType;
            }
        }
        
        public IdState()
        {
            State = false;
            LogicGates = new List<Gate>(); // <input id A, input id B, output bool O>
        }
    
        public void AddGate(Circuit.GateType gateType, int aId, int bId=-1)
        {
            LogicGates.Add(new Gate(aId, bId, gateType));
        }
    }
}