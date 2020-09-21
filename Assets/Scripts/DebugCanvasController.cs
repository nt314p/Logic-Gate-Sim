using System;
using LogicGateSimulator.PartBehaviors;
using TMPro;
using UnityEngine;

namespace LogicGateSimulator
{
    public class DebugCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI selectedPartsText;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private GameObject tooltipBackground;
        [SerializeField] private Transform tooltipTransform;
        [SerializeField] private SimulationManager simulationManager;
        private Vector3 _tooltipOffset;

        private void Awake()
        {
            var tooltipRect = tooltipBackground.GetComponent<RectTransform>().rect;
            _tooltipOffset = new Vector3(-tooltipRect.width/2, tooltipRect.height/2);
            simulationManager.MouseHoverOverPart += UpdatePartBehaviorHover;
        }

        private void Update()
        {
            var selectedText = "";
            var selectedParts = simulationManager.GetSelectedParts();
            foreach (var partBehavior in selectedParts)
            {
                selectedText += partBehavior.PartObject.ToString() + "\n";
            }

            selectedPartsText.text = selectedText;
            var mouseCoordinates = (Input.mousePosition);
            tooltipTransform.position = mouseCoordinates + _tooltipOffset;
        }
        
        private void UpdatePartBehaviorHover(PartBehavior partBehavior)
        {
            var isHovering = partBehavior != null;
            tooltipBackground.SetActive(isHovering);
            if (!isHovering)
            {
                tooltipText.text = "";
                return;
            }

            var partObject = partBehavior.PartObject;
            var text = "";
            text += $"Type: {partObject.GetType().Name}\n";
            text += $"Id: {partObject.Id}\n";
            text += $"State: {partObject.State}\n";
            text += $"Active: {partObject.Active}";
            tooltipText.text = text;
        }
    }
}
