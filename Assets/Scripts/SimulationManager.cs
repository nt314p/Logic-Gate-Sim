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
        private static SimulationManager _instance = null;
        public WireBehavior WirePrefabBehavior;
        public LedBehavior LedPrefabBehavior;
        public SwitchBehavior SwitchPrefabBehavior;
        public ButtonBehavior ButtonPrefabBehavior;
        private List<WireBehavior> _wiresInPath;
        private List<Vector2Int> _wirePath;
        public bool DrawingWirePath;
        private string _selectedPart;
        private Circuit _currentCircuit;
        private List<PartBehavior> _selectedParts;
        [SerializeField] private Camera _mainCamera;

        public Dictionary<KeyCode, string> Keybinds = new Dictionary<KeyCode, string>
        { { KeyCode.W, "Wire" },
            { KeyCode.L, "led" },
            { KeyCode.S, "switch" },
            { KeyCode.B, "button" }
        };

        // Start is called before the first frame update
        private void Start()
        {
            Application.targetFrameRate = 60;
            _instance = this;
            _wirePath = new List<Vector2Int>();
            _wiresInPath = new List<WireBehavior>();
            DrawingWirePath = false;
            _selectedPart = "";
            _selectedParts = new List<PartBehavior>();

            _currentCircuit = new Circuit(GridController.Width, GridController.Height);
            //_currentCircuit.RecalculateIds();
        }

        // Update is called once per frame
        private void Update()
        {
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

            // Debug.Log(selectedPart);

            var temp = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
                        AddPart(LedPrefabBehavior, mouseCoordinates);
                        break;
                    case "switch":
                        AddPart(SwitchPrefabBehavior, mouseCoordinates);
                        break;
                    case "button":
                        AddPart(ButtonPrefabBehavior, mouseCoordinates);
                        break;
                    default:
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
                            Destroy(_wiresInPath[_wiresInPath.Count - 1]); // removing Wire
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
                                var wireBehavior = Instantiate(WirePrefabBehavior, ToVector3(start),
                                    Quaternion.identity);
                                wireBehavior.PartObject = new Wire(start, end);
                                _wiresInPath.Add(wireBehavior);
                            }
                        }
                    }
                }
            }

            if (!Input.GetMouseButtonUp(0)) return;
            
            if (_wiresInPath.Count > 0)
                _currentCircuit.AddWires(_wiresInPath.Select(w => (Wire)w.PartObject).ToList());
            
            _wiresInPath.Clear(); // resetting variables
            _wirePath = new List<Vector2Int>();
            DrawingWirePath = false;
        }

        public void AddPart(PartBehavior partBehavior, Vector2Int coordinates)
        {
            if (partBehavior is WireBehavior) return;
            var instantiatedPartBehavior = Instantiate(partBehavior, ToVector3(coordinates), Quaternion.identity);
            var partType = instantiatedPartBehavior.PartType;
            instantiatedPartBehavior.PartObject = Activator.CreateInstance(partType) as Part;
            _currentCircuit.AddPart(instantiatedPartBehavior.PartObject);
        }

        public string GetSelectedPart()
        {
            return _selectedPart;
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

        public void ClearSelected()
        {
            while (_selectedParts.Count != 0)
            {
                _selectedParts[0].Selected = false;
            }
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
            var ret = new List<Vector2Int> {start}; // list of the coordinates of the steps

            var targetAngle = AngleOf(start, end); // determining angle of the diagonal line to approximate

            var steps = Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y); // compute total steps
            var signX = Mathf.RoundToInt(Mathf.Sign((end.x - start.x))); // which direction steps
            var signY = Mathf.RoundToInt(Mathf.Sign((end.y - start.y))); // go in (up down left right)

            for (var i = 0; i < steps; i++)
            { // stepping and incrementing
                var last = ret[ret.Count - 1];
                var vStep = new Vector2Int(last.x, last.y + signY); // computing both possible steps
                var hStep = new Vector2Int(last.x + signX, last.y);

                // the angle formed when the last coordinate is incremented by a vertical step
                var vAngle = AngleOf(start, vStep);
                // the angle formed when the last coordinate is incremented by a horizontal step
                var hAngle = AngleOf(start, hStep);

                // add the step that will bring us closer to the target angle
                ret.Add(Mathf.Abs(vAngle - targetAngle) < Mathf.Abs(hAngle - targetAngle) ? vStep : hStep);
            }
            ret.RemoveAt(0); // removing starting coordinate
            return ret;
        }

        private Vector2Int ClampedVectorInBounds(Vector2Int vector)
        {
            var clamped = new Vector2Int(vector.x, vector.y);
            clamped.Clamp(Vector2Int.zero, new Vector2Int(_currentCircuit.GridWidth - 1, _currentCircuit.GridHeight - 1));
            return clamped;
        }

        private bool IsWithinBounds(Vector2Int vector)
        {
            return vector.Equals(ClampedVectorInBounds(vector));
        }

        public static SimulationManager Sim()
        {
            return _instance;
        }

    }
}