using System;
using System.Collections.Generic;
using System.Linq;
using LogicGateSimulator.Circuits;
using LogicGateSimulator.PartBehaviors;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator
{
    public class SimulationManager : MonoBehaviour
    {
        private const int MaxMouseInputColliders = 7;
        [SerializeField] private WireBehavior wirePrefabBehavior;
        [SerializeField] private LedBehavior ledPrefabBehavior;
        [SerializeField] private SwitchBehavior switchPrefabBehavior;
        [SerializeField] private ButtonBehavior buttonPrefabBehavior;
        private List<WireBehavior> _wiresInPath;
        private List<Vector2Int> _wirePath;
        public bool DrawingWirePath;
        private string _selectedPart;
        private Circuit _currentCircuit;
        private Collider2D[] _mouseInputColliders;
        private PartBehavior _hoveringPart;
        private List<PartBehavior> _selectedParts;
        [SerializeField] private Camera mainCamera;
        public Camera MainCamera => mainCamera;
        public event Action<PartBehavior> MouseDownOnPart;
        public event Action<PartBehavior> MouseUpOnPart;
        public event Action<PartBehavior> MouseHoverOverPart;
        
        public Dictionary<KeyCode, string> Keybinds = new Dictionary<KeyCode, string>
        {
            {KeyCode.W, "Wire"},
            {KeyCode.L, "led"},
            {KeyCode.S, "switch"},
            {KeyCode.B, "button"}
        };

        // Start is called before the first frame update
        private void Start()
        {
            Application.targetFrameRate = 120;
            _wirePath = new List<Vector2Int>();
            _wiresInPath = new List<WireBehavior>();
            DrawingWirePath = false;
            _selectedPart = "";
            _selectedParts = new List<PartBehavior>();
            _currentCircuit = new Circuit(GridController.Width, GridController.Height);
            _mouseInputColliders = new Collider2D[MaxMouseInputColliders];

            MouseDownOnPart += OnMouseDownPart;
            MouseUpOnPart += OnMouseUpPart;
            MouseHoverOverPart += OnMouseHoverPart;
            //_currentCircuit.RecalculateIds();
        }

        // Update is called once per frame
        private void Update()
        {
            ProcessMouseInput();
            var pressed = false;
            foreach (var entry in Keybinds)
            {
                if (!Input.GetKey(entry.Key)) continue;
                _selectedPart = entry.Value;
                pressed = true;
            }

            if (!pressed)
            {
                _selectedPart = "";
            }
            
            var temp = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var mouseCoordinates = new Vector2Int(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
            mouseCoordinates = ClampedVectorInBounds(mouseCoordinates); // force clamp
            if (Input.GetMouseButtonDown(0))
            {
                switch (_selectedPart)
                {
                    case "Wire":
                        if (IsWithinBounds(mouseCoordinates))
                        {
                            _wirePath.Add(mouseCoordinates);
                            DrawingWirePath = true;
                        }

                        break;
                    case "led":
                        AddPart(ledPrefabBehavior, mouseCoordinates);
                        break;
                    case "switch":
                        AddPart(switchPrefabBehavior, mouseCoordinates);
                        break;
                    case "button":
                        AddPart(buttonPrefabBehavior, mouseCoordinates);
                        break;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (DrawingWirePath)
                {
                    var previousCoordinates = _wirePath[_wirePath.Count - 1];

                    // coordinate is unique and in grid bounds
                    if (!(mouseCoordinates.Equals(previousCoordinates)) && IsWithinBounds(mouseCoordinates))
                    {
                        // is the Wire going back on itself
                        if (_wirePath.Count >= 2 && _wirePath[_wirePath.Count - 2] == mouseCoordinates)
                        {
                            Destroy(_wiresInPath[_wiresInPath.Count - 1].gameObject);
                            _wirePath.RemoveAt(_wirePath.Count - 1);
                            _wiresInPath.RemoveAt(_wiresInPath.Count - 1);
                        }
                        else
                        {
                            // adding new Wire
                            var interpolated = Interpolate(previousCoordinates, mouseCoordinates);
                            foreach (var wireCoordinate in interpolated)
                            {
                                _wirePath.Add(wireCoordinate);
                                var start = _wirePath[_wirePath.Count - 2];
                                var end = _wirePath[_wirePath.Count - 1];
                                var wireBehavior = Instantiate(wirePrefabBehavior, ToVector3(start),
                                    Quaternion.identity);
                                wireBehavior.enabled = true;
                                wireBehavior.PartObject = new Wire(start, end);
                                SubscribeToPartBehaviorEvents(wireBehavior);
                                _wiresInPath.Add(wireBehavior);
                            }
                        }
                    }
                }
            }

            if (!Input.GetMouseButtonUp(0)) return;

            if (_wiresInPath.Count > 0)
            {
                var wiresList = _wiresInPath.Select(w => (Wire) w.PartObject).ToList();
                _currentCircuit.AddWires(wiresList, _wirePath);
            }

            _wiresInPath.Clear();
            _wirePath.Clear();
            DrawingWirePath = false;
        }

        private void ProcessMouseInput()
        {
            var mouseDown = Input.GetMouseButtonDown(0);
            
            var size = Physics2D.OverlapPointNonAlloc(mainCamera.ScreenToWorldPoint(Input.mousePosition), _mouseInputColliders);
            if (_mouseInputColliders.Length == 0) return;
            
            PartBehavior lastPartBehavior = null;

            if (_mouseInputColliders.Length != 1) // parts, not plane
            {
                for(var index = 0; index < size; index++)
                {
                    if (!_mouseInputColliders[index].TryGetComponent(out PartBehavior partBehavior)) continue;
                    lastPartBehavior = partBehavior;
                    if (!(lastPartBehavior is WireBehavior)) break;
                }
            }
            
            if (mouseDown)
                MouseDownOnPart?.Invoke(lastPartBehavior);
            else if(Input.GetMouseButtonUp(0))
                MouseUpOnPart?.Invoke(lastPartBehavior);
            else if (_hoveringPart != lastPartBehavior)
            {
                _hoveringPart = lastPartBehavior;
                MouseHoverOverPart?.Invoke(lastPartBehavior);
            }
        }

        private void OnMouseDownPart(PartBehavior partBehavior)
        {
            if (partBehavior == null)
            {
                DeselectAllParts();
                return;
            }
            var partObject = partBehavior.PartObject;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (partObject.Active) partObject.State = !partObject.State;
            }
            else if (_selectedPart == "")
            {
                SelectPart(partBehavior);
            }
        }

        private void OnMouseUpPart(PartBehavior partBehavior)
        {
            
        }

        private void OnMouseHoverPart(PartBehavior partBehavior)
        {
            // Debug.Log("Hovering over: " + partBehavior);
        }

        private void AddPart(PartBehavior partBehavior, Vector2Int coordinates)
        {
            if (partBehavior is WireBehavior) return;
            var instantiatedPartBehavior = Instantiate(partBehavior, ToVector3(coordinates), Quaternion.identity);
            var partType = instantiatedPartBehavior.PartType;
            instantiatedPartBehavior.PartObject = Activator.CreateInstance(partType) as Part;
            var instantiatedPart = instantiatedPartBehavior.PartObject;
            instantiatedPart.Coordinates = coordinates;
            _currentCircuit.AddPart(instantiatedPart);
            SubscribeToPartBehaviorEvents(instantiatedPartBehavior);
        }

        private void SubscribeToPartBehaviorEvents(PartBehavior partBehavior)
        {
            //partBehavior.SelectChanged += UpdateSelectedPart;
            //partBehavior.MouseHover += DebugCanvasController.UpdatePartBehaviorHover;
        }

        private void SelectPart(PartBehavior partBehavior)
        {
            partBehavior.Selected = !partBehavior.Selected;
            if (partBehavior.Selected)
            {
                if (_selectedParts.Contains(partBehavior)) return;
                _selectedParts.Add(partBehavior);
            }
            else
                _selectedParts.Remove(partBehavior);
        }

        private void DeselectAllParts()
        {
            foreach (var partBehavior in _selectedParts)
            {
                partBehavior.Selected = false;
            }
            _selectedParts.Clear();
        }

        public List<PartBehavior> GetSelectedParts()
        {
            return _selectedParts;
        }

        public Circuit GetCircuit()
        {
            return _currentCircuit;
        }

        public bool ToggleSelected(PartBehavior p)
        {
            if (_selectedParts.Contains(p))
            {
                _selectedParts.Remove(p);
            }
            else
            {
                _selectedParts.Add(p);
            }

            return !_selectedParts.Contains(p);
        }
        
        private static Vector3 ToVector3(Vector2Int vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0);
        }

        // returns the angle of the vector formed by the vector2s passed in
        private static float AngleOf(Vector2Int start, Vector2Int end)
        {
            var angle = Mathf.Atan2((start.y - end.y), (start.x - end.x)) * Mathf.Rad2Deg;
            if (angle < 0) angle += 180; // we want a positive angle
            return angle % 180;
        }

        // Interpolate method determines the horizontal and vertical steps to model a diagonal line
        private static List<Vector2Int> Interpolate(Vector2Int start, Vector2Int end)
        {
            var interpolated = new List<Vector2Int> {start}; // list of the coordinates of the steps

            var targetAngle = AngleOf(start, end); // determining angle of the diagonal line to approximate

            var steps = Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y); // compute total steps
            var signX = Mathf.RoundToInt(Mathf.Sign((end.x - start.x))); // which direction steps
            var signY = Mathf.RoundToInt(Mathf.Sign((end.y - start.y))); // go in (up down left right)

            for (var index = 0; index < steps; index++)
            {
                var last = interpolated[interpolated.Count - 1];
                var verticalStep = new Vector2Int(last.x, last.y + signY);
                var horizontalStep = new Vector2Int(last.x + signX, last.y);

                var verticalAngle = AngleOf(start, verticalStep);
                var horizontalAngle = AngleOf(start, horizontalStep);

                // add the step that will bring us closer to the target angle
                interpolated.Add(Mathf.Abs(verticalAngle - targetAngle) < Mathf.Abs(horizontalAngle - targetAngle) ? verticalStep : horizontalStep);
            }

            interpolated.RemoveAt(0); // removing starting coordinate
            return interpolated;
        }

        private Vector2Int ClampedVectorInBounds(Vector2Int vector)
        {
            var clamped = new Vector2Int(vector.x, vector.y);
            clamped.Clamp(Vector2Int.zero,
                new Vector2Int(_currentCircuit.GridWidth - 1, _currentCircuit.GridHeight - 1));
            return clamped;
        }

        private bool IsWithinBounds(Vector2Int vector)
        {
            return vector.Equals(ClampedVectorInBounds(vector));
        }
    }
}