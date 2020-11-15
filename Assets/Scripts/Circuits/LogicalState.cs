using System.Collections.Generic;

namespace LogicGateSimulator.Circuits
{
    public class LogicalState
    {
        private IdState[] masterState;

        public LogicalState(int idCount)
        {
            masterState = new IdState[idCount];
            // foreach (var idState in masterState)
            // {
            //     idState.gateEvaluations = new List<int[]>[4];
            // }
        }


    }
}