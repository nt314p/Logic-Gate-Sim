using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PartGrid
{
    private readonly PartWrapper[,] _partGrid;
    private readonly IDictionary<int, List<Part>> _partsDictionary;

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
        foreach (var wire in wires)
        {
            AddWire(wire, false); // don't add to dictionary because it will be added at the very end
            var surroundingWires = GetWiresFromDirection(wire.Coordinates, wire.Orientation);
        }
    }

    public void AddWire(Wire wire, bool addToDictionary = true)
    {
        GetWrapper(wire.Coordinates).SetWire(wire);
        UpdateConnection(wire);
        if (addToDictionary)
        {

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

    public List<Wire> GetWiresFromDirection(Vector2Int coordinates, Vector2Int direction, bool ignoreConnected = false)
    {
        if (!ignoreConnected && !IsConnected(coordinates)) return new List<Wire> { GetWire(coordinates, -direction) };
        var wires = new List<Wire>();
        for (var i = 0; i < 3; i++)
        {
            direction = new Vector2Int(-direction.y, direction.x); // rotate vector 90 deg CCW
            var wire = GetWire(coordinates, direction);
            if (wire != null && wire.Id != -1) wires.Add(wire); // ignore wires with id -1 (unregistered)
        }

        return wires;
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
