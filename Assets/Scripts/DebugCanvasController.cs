using TMPro;
using UnityEngine;

namespace LogicGateSimulator
{
    public class DebugCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _selectedPartsText;
        [SerializeField] private SimulationManager _simulationManager;

        void Update()
        {
            var selectedText = "";
            var selectedParts = _simulationManager.GetSelectedParts();
            foreach (var partBehavior in selectedParts)
            {
                selectedText += partBehavior.PartObject.ToString() + "\n";
            }

            _selectedPartsText.text = selectedText;
        }
    }
}
