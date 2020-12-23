using System;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;

namespace LogicGateSimulator.Circuits
{
    public class Circuit
    {
        public IdState[] circuitState;

        public enum GateType
        {
            And,
            Or,
            Not,
            Xor
        }

        public void EvaluateCircuit() // can lead to race conditions
        {
            for (var index = 0; index < circuitState.Length; index++)
            {
                circuitState[index].State = EvaluateId(index);
            }
        }

        private bool EvaluateId(int id) // should this return IF the value changed instead of the value?
        {
            var idState = circuitState[id];

            foreach (var gate in idState.LogicGates)
            {
                if (EvaluateGate(gate))
                {
                    idState.State = true;
                    return true;
                }
            }

            // found no true values, therefore false
            idState.State = false;
            return false;
        }

        private bool EvaluateGate(IdState.Gate gate)
        {
            switch (gate.GateType)
            {
                case GateType.And:
                    return circuitState[gate.AId].State & circuitState[gate.BId].State;
                case GateType.Or:
                    return circuitState[gate.AId].State | circuitState[gate.BId].State;
                case GateType.Not:
                    return !circuitState[gate.AId].State;
                case GateType.Xor:
                    return circuitState[gate.AId].State ^ circuitState[gate.BId].State;
                default:
                    return false;
            }
        }
    }
}