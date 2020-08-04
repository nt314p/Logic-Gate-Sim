using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Circuit
{

	private readonly PartWrapper[,] _partsGrid;
	private IDictionary<int, List<Part>> _parts;
	private int _nextId;

	public Circuit(int gridX, int gridY)
	{
		_parts = new Dictionary<int, List<Part>>();
		_partsGrid = new PartWrapper[gridX, gridY];
		for (var i = 0; i < _partsGrid.GetLength(0); i++)
		{
			for (var j = 0; j < _partsGrid.GetLength(1); j++)
			{
				_partsGrid[i, j] = new PartWrapper();
			}
		}
		_nextId = 0;
	}

	public void RebuildIds()
	{ // full rebuild
	  // assume that parts dictionary (which contains ids and lists of parts) is valid
		var allParts = _parts.SelectMany(x => x.Value).ToList();
		allParts.ForEach(p => p.Id = -1); // clearing ids

		// foreach ()
	}

	public void RecalculateIds()
	{
		Debug.Log("Recalculating!");
		_parts = new Dictionary<int, List<Part>>();
		_nextId = 0;
		for (var i = 0; i < _partsGrid.GetLength(0); i++)
		{ // clearing ids
			for (var j = 0; j < _partsGrid.GetLength(1); j++)
			{
				for (var k = 0; k < 2; k++)
				{
					if (_partsGrid[i, j].GetParts()[k] != null)
					{
						_partsGrid[i, j].GetParts()[k].Id = -1;
					}
				}
			}
		}

		for (var i = 0; i < _partsGrid.GetLength(0); i++)
		{
			for (var j = 0; j < _partsGrid.GetLength(1); j++)
			{ // iterating through 2d grid
				var left = GetWire(new Vector2Int(i, j), Vector2Int.left);
				var bottom = GetWire(new Vector2Int(i, j), Vector2Int.down);

				int nodeId; // the id at the node (i, j)
				if (left == null && bottom == null)
				{
					nodeId = _nextId;
				}
				else if (left != null && bottom != null)
				{
					nodeId = left.Id;
					var bottomPathParts = GetAllOfId(bottom.Id); // getting bottom path
					for (var k = 0; k < bottomPathParts.Count; k++)
					{ // iterating through bottom path parts
						var p = bottomPathParts[k];

						// setting bottom path ids to left path ids
						if (UpdatePartIdInDict(p, nodeId))
						{
							// return is true, a part was moved and not created
							k--;
						}
					}
				}
				else if (left == null)
				{ // left null, bottom not null
					nodeId = bottom.Id;
				}
				else
				{ // left not null, bottom null
					nodeId = left.Id;
				}

				var nodeParts = _partsGrid[i, j].GetParts();

				foreach (var p in nodeParts)
				{
					if (p != null)
					{
						UpdatePartIdInDict(p, nodeId);
					}
				}

				var top = _partsGrid[i, j].GetWire(Vector2Int.up);
				var right = _partsGrid[i, j].GetWire(Vector2Int.right);

				if (top != null || right != null)
				{
					_nextId++;
				}
			}
		}

		TrimIds();

		var ids = new int[_parts.Keys.Count];
		_parts.Keys.CopyTo(ids, 0);
		foreach (var currentId in ids)
		{ // recalculating states for all ids
			CalculateStateId(currentId);
		}
	}

	public void TrimIds()
	{
		_nextId = 0;
		int[] ids = new int[_parts.Keys.Count];
		_parts.Keys.CopyTo(ids, 0);
		Debug.Log("Trimming");
		foreach (var currentId in ids)
		{
			if (_parts[currentId].Count == 0)
			{
				_parts.Remove(currentId);
			}
			else
			{
				ReplaceId(currentId, _nextId++);
			}
		}
	}

	// calculates the state of an id based on the states of its active components
	public void CalculateStateId(int id)
	{
		var state = false;
		foreach (var p in _parts[id])
		{
			if (!p.Active || state) break; // end of active parts or state = true -> can't be made false
			state |= p.State;
		}
		SetAllOfId(id, state);
	}

	public void ReplaceId(int oldId, int newId)
	{
		if (oldId == newId) return;
		_parts[newId] = _parts[oldId]; // set list reference in dict to new id
		_parts.Remove(oldId);
		_parts[newId].ForEach(p => p.Id = newId);
	}

	// links all part ids passed in
	public void LinkIds(Part[] parts)
	{
		int[] validPartIds = parts.Where(p => p != null).Select(p => p.Id).ToArray();
		for (int i = 1; i < validPartIds.Length; ++i)
		{
			ReplaceId(validPartIds[i], validPartIds[0]); // link other ids to first id
		}
	}

	public Wire[] GetWiresFromDirection(Vector2Int coords, Vector2Int direction, bool ignoreConnected)
	{
		float angle = Mathf.Atan2(direction.y, direction.x);
		return null;
	}

	public List<Part> GetAllOfId(int id)
	{
		return _parts[id];
	}

	// hard set all, sets ALL components, soft set will ignore active components
	public void SetAllOfId(int id, bool state, bool hardSet = false)
	{
		List<Part> editParts = GetAllOfId(id);
		if (!hardSet)
		{ // soft set filters out active parts
			editParts = editParts.Where(p => !p.Active).ToList();
		}
		editParts.ForEach(p => p.State = state);
	}

	public void AddNode(GameObject nodeObj, Vector2Int coords)
	{
		GameObject tempObj = MonoBehaviour.Instantiate(nodeObj, ToVector3(coords), Quaternion.identity);
		Part nodePart = tempObj.GetComponent<Part>();
		nodePart.Coords = coords;
		nodePart.Id = _nextId;
		AddPart(nodePart);
		_nextId++;
		RecalculateIds();
	}

	public void AddWires(List<Wire> wires)
	{
		// wires in list are not yet in 2d grid



		foreach (Wire w in wires)
		{
			w.Id = _nextId;
			AddPart(w);
		}

		if (wires.Count > 0)
		{
			_nextId++;
			RecalculateIds();
		}
	}

	private void AddPart(Part p)
	{
		if (p == null) return;
		Vector2Int coords = p.Coords;
        if (p is Wire w)
        {
            _partsGrid[coords.x, coords.y].SetWire(w, w.GetOrientation());
        }
        else
        {
            _partsGrid[coords.x, coords.y].SetNode(p);
            Debug.Log("Added " + p.GetType().ToString());
        }

        /*
        if (p is LED) {
            LED led = (LED) p;
            partsGrid[coords.x, coords.y].SetNode (led);
            Debug.Log ("ADDED AN LED!");
        } else if (p is Switch) {
            Switch sw = (Switch) p;
            partsGrid[coords.x, coords.y].SetNode (sw);
            Debug.Log ("ADDED A SWITCH!");
        } else if (p is Button) {
            Button b = (Button) p;
            partsGrid[coords.x, coords.y].SetNode (b);
            Debug.Log ("ADDED A BUTTON!");
        }*/
        AddPartToDict(p);
	}

	private void AddPartToDict(Part part)
	{
		int partId = part.Id;
		if (partId == -2 || partId == -1)
		{
			Debug.Log("POTATS! partId is: " + partId);
		}
		List<Part> partsOfId = new List<Part>();
		if (_parts.ContainsKey(partId))
		{ // key (id) exists
			partsOfId = _parts[partId];
		}
		else
		{ // key (id) doesn't exist, initialize list
			_parts.Add(partId, partsOfId);
		}
		if (part.Active)
		{
			partsOfId.Insert(0, part); // inserting active part at the front
		}
		else
		{
			partsOfId.Add(part); // adding passive part to the end
		}
	}

	private bool UpdatePartIdInDict(Part part, int newId)
	{
		bool idExists = _parts.ContainsKey(part.Id);
		if (idExists)
		{
			List<Part> partsOfId = _parts[part.Id];
			partsOfId.Remove(part);
		}
		part.Id = newId;
		AddPartToDict(part);
		return idExists && part.Id != newId; // prevent infinite loops
	}

	private static Vector3 ToVector3(Vector2Int vecInt)
	{
		return (Vector3)(Vector2)vecInt;
	}

	// gets a wire based on direction from the part grid
	private Wire GetWire(Vector2Int coord, Vector2Int direction)
	{
		if (coord.x == 0 || coord.y == 0) return null;
		if (direction.x == -1 || direction.y == -1)
		{ // left or bottom
			Vector2Int shifted = coord + direction;
			return _partsGrid[shifted.x, shifted.y].GetWire(-direction);
		}
		else
		{ // top or right
			return _partsGrid[coord.x, coord.y].GetWire(direction);
		}
	}
	/*
    private Part GetNode(Vector2Int coord) {
        if (Vector2.ang)
        try {
            return partsGrid[coord.x, coord.y].GetNode();
        } catch (IndexOutOfRangeException e) {
            return null;
        }
    }

    // returns the id of the coorinate, if it exists
    private int GetCoordId(Vector2Int coord) {
        Part node = GetNode(coord);
        if (node != null) return node.Id; // might only need this line
        // In the future, a WireConnectionNode part might exist which would
        // indicate that the horizontal and vertical wires are connected
        // it would share the same id as those wires, and thus make it redundent
        // to check the wires.

    }*/
}