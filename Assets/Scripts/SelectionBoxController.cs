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
        private Vector3 _startDragCoordinates;
        private bool _isDragging;
        
        
        // Start is called before the first frame update
        private void Start()
        {
            _mainCamera = simulationManager.MainCamera;
            selectionBoxImage.enabled = false;
            _isDragging = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (false && Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                var hitCollider = hit.collider;
                if (hitCollider != null && !hitCollider.CompareTag("Part"))
                {
                    var coords = Input.mousePosition;
                    selectionBoxImage.enabled = true;
                    selectionBoxRect.position = coords;
                    Debug.Log(coords);
                }
            }
        }
    }
}
