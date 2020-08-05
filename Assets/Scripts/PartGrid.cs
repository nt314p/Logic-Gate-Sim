using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartGrid
{
    private readonly PartWrapper[,] _partGrid;
    private readonly IDictionary<int, List<Part>> _parts;

    public PartGrid(int gridWidth, int gridHeight)
    {
        _parts = new Dictionary<int, List<Part>>();
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
        var ids = new int[_parts.Keys.Count];
        _parts.Keys.CopyTo(ids, 0);
        Debug.Log("Trimming Ids");
        foreach (var currentId in ids)
        {
            if (_parts[currentId].Count == 0)
            {
                _parts.Remove(currentId);
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
        _parts[newId] = _parts[oldId]; // set list reference in dict to new id
        _parts.Remove(oldId);
        _parts[newId].ForEach(p => p.Id = newId);
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

    public Wire[] GetWiresFromDirection(Vector2Int coordinates, Vector2Int direction, bool ignoreConnected)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        return null;
    }

    public List<Part> GetPartsOfId(int id) => _parts[id];

    public void SetPartsOfId(int id, bool state, bool hardSet = false)
    {
        var editParts = GetPartsOfId(id);
        if (!hardSet) editParts = editParts.Where(p => !p.Active).ToList(); // soft set filters out active parts
        editParts.ForEach(p => p.State = state);
    }

    // gets a Wire based on direction from the part grid
    private Wire GetWire(Vector2Int coordinate, Vector2Int direction)
    {
        if (coordinate.x == 0 || coordinate.y == 0) return null;
        if (direction.x == -1 || direction.y == -1)
        { // left or bottom
            var shifted = coordinate + direction;
            return _partGrid[shifted.x, shifted.y].GetWire(-direction);
        }
        else
        { // top or right
            return _partGrid[coordinate.x, coordinate.y].GetWire(direction);
        }
    }

    private Part GetNode(Vector2Int coordinate)
    {
        return _partGrid[coordinate.x, coordinate.y].GetNode();
    }
}
