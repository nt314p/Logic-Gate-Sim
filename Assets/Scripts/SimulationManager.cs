using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimulationManager : MonoBehaviour
{
    private static SimulationManager _instance = null;
    public static GameObject Wire;
    public static GameObject Led;
    public static GameObject Switch;
    public static GameObject Button;
    private List<GameObject> _wiresInPath;
    private List<Vector2Int> _wirePath;
    public bool DrawingWirePath;
    private string _selectedPart;
    private Circuit _currentCircuit;
    private List<PartBehavior> _selectedParts;

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
        Wire = Resources.Load("Prefabs/Wire") as GameObject;
        Led = Resources.Load("Prefabs/LED") as GameObject;
        Switch = Resources.Load("Prefabs/Switch") as GameObject;
        Button = Resources.Load("Prefabs/Button") as GameObject;

        _wirePath = new List<Vector2Int>();
        _wiresInPath = new List<GameObject>();
        DrawingWirePath = false;
        _selectedPart = "";
        _selectedParts = new List<PartBehavior>();

        _currentCircuit = new Circuit(GridController.Width, GridController.Height);
        _currentCircuit.RecalculateIds();
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

        var temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouseCoordinates = new Vector2Int(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));

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
                    _currentCircuit.AddNode(Led, mouseCoordinates);
                    break;
                case "switch":
                    _currentCircuit.AddNode(Switch, mouseCoordinates);
                    break;
                case "button":
                    _currentCircuit.AddNode(Button, mouseCoordinates);
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
                            _wiresInPath.Add(Instantiate(Wire, ToVector3(start), Quaternion.identity));
                            _wiresInPath[_wiresInPath.Count - 1].GetComponent<WireBehavior>();//.Initialize(start, end);
                        }
                    }
                }
            }
        }

        if (!Input.GetMouseButtonUp(0)) return;

        var wireList = new List<WireBehavior>();
        foreach (var wireBehavior in _wiresInPath)
        {
            wireList.Add(wireBehavior.GetComponent<WireBehavior>());
        }
        if (wireList.Count > 0)
            _currentCircuit.AddWires(wireList.Select(w => (Wire)w.PartObject).ToList());
        _wiresInPath = new List<GameObject>(); // resetting variables
        _wirePath = new List<Vector2Int>();
        DrawingWirePath = false;

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

    private static bool IsWithinBounds(Vector2Int vec)
    {
        var clamped = new Vector2Int(vec.x, vec.y);
        clamped.Clamp(Vector2Int.zero, new Vector2Int(49, 49)); // fix hardcode
        return clamped.Equals(vec);
    }

    public static SimulationManager Sim()
    {
        return _instance;
    }

}