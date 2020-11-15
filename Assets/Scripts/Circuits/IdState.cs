using System.Collections.Generic;

namespace LogicGateSimulator.Circuits
{
    public class IdState
    {
        private const int GateCount = 4;
        private bool state;
        private List<int[]>[] gateEvaluations;
        
        private enum Gates
        {
            And,
            Or,
            Not,
            Xor
        }

        public IdState()
        {
            gateEvaluations = new List<int[]>[GateCount];
            for (var index = 0; index < GateCount; index++)
            {
                gateEvaluations[index] = new List<int[]>();
            }
        }
    }
}