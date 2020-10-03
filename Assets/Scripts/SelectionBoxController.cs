using LogicGateSimulator.PartBehaviors;
using UnityEngine;
using UnityEngine.UI;


namespace LogicGateSimulator
{
    
    public class SelectionBoxController : MonoBehaviour
    {
        [SerializeField] private RectTransform selectionBoxRect;
        [SerializeField] private Image selectionBoxImage;
        [SerializeField] private SimulationManager simulationManager;
        private Camera _mainCamera;
        private Vector2 _startDragCoordinates;
        private bool _isDragging;

        private void Start()
        {
            _mainCamera = simulationManager.MainCamera;
            simulationManager.MouseDownOnPart += OnMouseDownPart;
            simulationManager.MouseUpOnPart += OnMouseUpPart;
            selectionBoxImage.enabled = false;
            _isDragging = false;
        }

        // Update is called once per frame
        private void Update()
        {

            if (_isDragging)
            {
                var deltaPosition = (Vector2) _mainCamera.ScreenToWorldPoint(Input.mousePosition) - _startDragCoordinates;
                selectionBoxRect.position = _startDragCoordinates + deltaPosition * 0.5f;
                selectionBoxRect.sizeDelta = deltaPosition * 10f / 3;
            }
        }

        private void OnMouseDownPart(PartBehavior partBehavior)
        {
            _startDragCoordinates = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _isDragging = true;
            selectionBoxImage.enabled = true;
            
        }

        private void OnMouseUpPart(PartBehavior partBehavior)
        {
            _isDragging = false;
            selectionBoxImage.enabled = false;
        }
    }
}
