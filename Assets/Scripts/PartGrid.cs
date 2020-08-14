using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartGrid
{
    private readonly PartWrapper[,] _partGrid;
    private readonly IDictionary<int, List<Part>> _partsDictionary;
    private static readonly List<Vector2Int> Directions = new List<Vector2Int> { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
    private int nextAvailableId;

    public PartGrid(int gridWidth, int gridHeight)
    {
        nextAvailableId = 0;
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

    private void ReplaceId(int oldId, int newId)
    {
        if (oldId == newId) return;
        _partsDictionary[newId] = _partsDictionary[oldId];
        _partsDictionary.Remove(oldId);
        _partsDictionary[newId].ForEach(p => p.Id = newId);
    }

    private void MergeId(int sourceId, int destinationId)
    {
        if (sourceId == destinationId) return;
        _partsDictionary[destinationId].AddRange(_partsDictionary[sourceId]);
        _partsDictionary.Remove(sourceId);
        _partsDictionary[destinationId].ForEach(p => p.Id = destinationId);
    }

    // links all part ids passed in
    public void LinkPartsIds(Part[] parts)
    {
        var validPartIds = parts.Where(p => p != null).Select(p => p.Id).ToArray();
        for (var i = 1; i < validPartIds.Length; ++i)
        {
            ReplaceId(validPartIds[i], validPartIds[0]); // link other ids to first id
        }
    }

    private int LinkIds(List<int> ids)
    {
        var finalId = ids.Count == 0 ? GetAndIncrementNextAvailableId() : ids[0];
        for (var index = 1; index < ids.Count; index++)
        {
            MergeId(ids[index], finalId);
        }
        return finalId;
    }
    
    public void AddPart(Part part)
    {
        if (part is Wire wire)
        {
            AddWires(new List<Wire> {wire});
        }
        else
        {
            AddNode(part);
        }
    }

    private void AddNode(Part part)
    {
        var wrapper = GetWrapper(part.Coordinates);
        if (wrapper.Node != null) return; // implement throwing exception
        var surroundingWires = GetWiresAtCoordinates(part.Coordinates).Where(wire => wire != null).ToList();
        if (surroundingWires.Count == 4 && !wrapper.Connected) // ids need to be recalculated
        {
            var newId = LinkIds(wrapper.GetWires().Select(wire => wire.Id).ToList());
            part.Id = newId;
        }
        else
        {
            part.Id = GetAndIncrementNextAvailableId();
        }
        wrapper.Node = part;
        wrapper.Connected = true; // a node will always connect
    }

    public void AddWires(List<Wire> wires) // wires should be passed in with id -1, also should be in order
    {
        foreach (var wire in wires)
        {
            GetWrapper(wire.Coordinates).SetWire(wire); // adding part to the grid
            UpdateConnection(wire);
        }

        var firstWire = wires[0];
        var previousCoordinates = firstWire.Coordinates;
        var connectedIds = new HashSet<int>(); // using a set for unique ids only

        // adding the first wire's opposite direction since the for loop traversal moves in forward direction only
        var firstWireConnections =
            GetWiresFromDirection(firstWire.EndPoint, firstWire.Coordinates - firstWire.EndPoint);
        firstWireConnections.ForEach(surroundingWire => connectedIds.Add(surroundingWire.Id));

        for (var index = 1; index < wires.Count; index++)
        {
            var currentCoordinates = wires[index].Coordinates;
            var surroundingWires = GetWiresFromDirection(previousCoordinates, currentCoordinates - previousCoordinates);
            surroundingWires.ForEach(surroundingWire => connectedIds.Add(surroundingWire.Id));
            previousCoordinates = currentCoordinates;
        }

        var wireId = LinkIds(connectedIds.ToList());

        foreach (var wire in wires)
        {
            wire.Id = wireId;
            AddPartToDictionary(wire);
        }
    }

    private void AddPartToDictionary(Part part)
    {
        var partId = part.Id;
        var partsOfId = new List<Part>();
        if (_partsDictionary.ContainsKey(partId))
            partsOfId = _partsDictionary[partId];
        else
            _partsDictionary.Add(partId, partsOfId);
        
        if (part.Active)
            partsOfId.Insert(0, part); // active parts go in front
        else
            partsOfId.Add(part);

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
    private List<Wire> GetWiresFromDirection(Vector2Int coordinates, Vector2Int direction, bool ignoreConnected = false)
    {
        var wires = new List<Wire>();

        if (!ignoreConnected && !IsConnected(coordinates))
        {
            wires.Add(GetWire(coordinates, -direction));
        }
        else
        {
            for (var directionIndex = RotateQuarter(direction);
                directionIndex != direction;
                directionIndex = RotateQuarter(directionIndex))
            {
                wires.Add(GetWire(coordinates, directionIndex));
            }
        }

        // ignore wires that are null and with with id -1 (unregistered)
        return wires.Where(wire => wire != null && wire.Id != -1).ToList();
    }

    private List<Part> GetPartsOfId(int id) => _partsDictionary[id];

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
        return Directions.Select(direction => GetWire(coordinates, direction)).ToList();
    }

    // gets a Wire based on direction from the part grid
    private Wire GetWire(Vector2Int coordinates, Vector2Int direction)
    {
        if (coordinates.x == 0 || coordinates.y == 0) return null;
        if (direction.x == -1 || direction.y == -1) // left or bottoms
        { 
            var shifted = coordinates + direction;
            return GetWrapper(shifted).GetWire(-direction);
        }

        return GetWrapper(coordinates).GetWire(direction); // top or right
    }

    private PartWrapper GetWrapper(Vector2Int coordinates)
    {
        return _partGrid[coordinates.x, coordinates.y];
    }

    private int GetAndIncrementNextAvailableId()
    {
        return nextAvailableId++;
    }

    private static Vector2Int RotateQuarter(Vector2Int vector)
    {
        return new Vector2Int(-vector.y, vector.x);
    }
}
