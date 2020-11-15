
namespace LogicGateSimulator.Circuits
{
    public class Circuit
    {
        private StateIds[] circuitState;
        private struct StateIds
        {
            public bool state;
            public int[][] andIds;
            public int[][] orIds;
            public int[][] notIds;
            // public int[] xorIds;
        }

        public void EvaluateCircuit()
        {
            for (int index = 0; index < circuitState.Length; index++)
            {
                circuitState[index].state = EvaluateId(index);
            }
        }

        private bool EvaluateId(int id)
        {
            var stateIds = circuitState[id];
            var andEvaluation = true;
            if (!stateIds.state)
            {
                foreach (var orId in stateIds.orIds)
                {
                    if (circuitState[orId].state)
                        return true;
                }

                foreach (var andId in stateIds.andIds) // incorrect evaluation, evaluate in pairs (A, B)
                {
                    if (!circuitState[andId].state)
                    {
                        andEvaluation = false;
                        break;
                    }
                }

                if (andEvaluation) return true;
                
                foreach (var notId in stateIds.notIds)
                {
                    if (!circuitState[notId].state)
                        return true;
                }
                
                
                return false;
            }
        }
        
    }
}