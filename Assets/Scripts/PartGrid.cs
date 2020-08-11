using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartGrid
{
    private readonly PartWrapper[,] _partGrid;
    private readonly IDictionary<int, List<Part>> _partsDictionary;
    private static readonly List<Vector2Int> Directions = new List<Vector2Int> { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    public PartGrid(int gridWidth, int gridHeight)
    {
        _partsDictionary = new Dictionary<int, List<Part>>();
        _partGrid = new PartWrapper[gridWidth, gridHeight];
        for (var i = 0; i < _partGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _partGrid.GetLength(1); j++)
            {
                _partGrid[i, j] = new PartWrapper();
            }
        }
    }

    public void TrimIds()
    {
        var nextId = 0;
        var ids = new int[_partsDictionary.Keys.Count];
        _partsDictionary.Keys.CopyTo(ids, 0);
        Debug.Log("Trimming Ids");
        foreach (var currentId in ids)
        {
            if (_partsDictionary[currentId].Count == 0)
            {
                _partsDictionary.Remove(currentId);
            }
            else
            {
                ReplaceId(currentId, nextId++);
            }
        }
    }

    public void ReplaceId(int oldId, int newId) // Will overwrite parts that have id newId...
    {
        if (oldId == newId) return;
        _partsDictionary[newId] = _partsDictionary[oldId]; // set list reference in dict to new id
        _partsDictionary.Remove(oldId);
        _partsDictionary[newId].ForEach(p => p.Id = newId);
    }

    // links all part ids passed in
    public void LinkIds(Part[] parts)
    {
        var validPartIds = parts.Where(p => p != null).Select(p => p.Id).ToArray();
        for (var i = 1; i < validPartIds.Length; ++i)
        {
            ReplaceId(validPartIds[i], validPartIds[0]); // link other ids to first id
        }
    }

    public void Add(Part part)
    {
        if (part is Wire)
        {

        }
    }

    public void AddWires(List<Wire> wires) // wires should be passed in with id -1
    {
        var connectedIds = new HashSet<int>(); // using a set for unique ids only
        foreach (var wire in wires)
        {
            GetWrapper(wire.Coordinates).SetWire(wire);
            UpdateConnection(wire);
            var surroundingWires = GetWiresFromDirection(wire.Coordinates, wire.Orientation);
            surroundingWires.ForEach(surroundingWire => connectedIds.Add(surroundingWire.Id)); // add

        }
    }


    private void UpdateConnection(Wire addedWire)
    {
        var wireCount = 0;
        var coordinates = addedWire.Coordinates;
        var direction = Vector2Int.right;
        for (var i = 0; i < 4; i++)
        {
            direction = new Vector2Int(-direction.y, direction.x);
            if (GetWire(coordinates, direction) != null) wireCount++;
        }
        // Special case: three wires -> four wires remain connected (unless the wire opposite to the wire being added is also unregistered)
        switch (wireCount)
        {
            case 0:
            case 1:
            case 2:
                GetWrapper(coordinates).Connected = false;
                break;
            case 3:
                GetWrapper(coordinates).Connected = true;
                break;
            case 4:
                // if the node was connected and the opposing wire to the wire being added is not unregistered
                if (IsConnected(coordinates) && GetWire(coordinates, -addedWire.Orientation).Id != -1)
                {
                    break; // do nothing because the node should keep it's connection
                }
                GetWrapper(coordinates).Connected = false;
                break;
        }
    }

    /*
    private bool ContainsUnregisteredLine(Vector2Int coordinates)
    {
        bool horizontal = GetWire(coordinates, Vector2Int.right).Id == -1 && GetWire(coordinates, Vector2Int.left).Id == -1;
        bool vertical = GetWire(coordinates, Vector2Int.up).Id == GetWire(coordinates, Vector2Int.left).Id;
    }
    */
    // replace coordinates and direction with just wire
    public List<Wire> GetWiresFromDirection(Vector2Int coordinates, Vector2Int direction, bool ignoreConnected = false)
    {
        var wires = new List<Wire>();

        if (!ignoreConnected && !IsConnected(coordinates))
        {
            wires.Add(GetWire(coordinates, -direction));
        }
        else
        {
            GetWiresAtCoordinates(coordinates);
        }
        // ignore wires that are null and with with id -1 (unregistered)
        return wires.Where(wire => wire != null && wire.Id != -1).ToList();
    }

    public List<Part> GetPartsOfId(int id) => _partsDictionary[id];

    public void SetPartsOfId(int id, bool state, bool hardSet = false)
    {
        var editParts = GetPartsOfId(id);
        if (!hardSet) editParts = editParts.Where(p => !p.Active).ToList(); // soft set filters out active parts
        editParts.ForEach(p => p.State = state);
    }

    private bool IsConnected(Vector2Int coordinates)
    {
        return GetWrapper(coordinates).Connected;
    }

    private List<Wire> GetWireLine(Vector2Int coordinates, Vector2Int direction)
    {
        return new List<Wire> { GetWire(coordinates, direction), GetWire(coordinates, -direction) };
    }

    private List<Wire> GetWiresAtCoordinates(Vector2Int coordinates)
    {
        return Directions.Select(direction => (GetWire(coordinates, direction))).ToList();
    }

    // gets a Wire based on direction from the part grid
    private Wire GetWire(Vector2Int coordinates, Vector2Int direction)
    {
        if (coordinates.x == 0 || coordinates.y == 0) return null;
        if (direction.x == -1 || direction.y == -1)
        { // left or bottom
            var shifted = coordinates + direction;
            return GetWrapper(shifted).GetWire(-direction);
        }

        // top or right
        return GetWrapper(coordinates).GetWire(direction);
    }

    private Part GetNode(Vector2Int coordinates)
    {
        return GetWrapper(coordinates).Node;
    }

    private PartWrapper GetWrapper(Vector2Int coordinates)
    {
        return _partGrid[coordinates.x, coordinates.y];
    }
}
