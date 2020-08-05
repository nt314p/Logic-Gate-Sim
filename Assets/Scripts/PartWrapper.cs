using System.Collections.Generic;
using UnityEngine;

public class PartWrapper
{

	private readonly Dictionary<Vector2Int, Wire> _wires;
	private Part _node;
	private bool _isConnected; // whether or not the horizontal and vertical wires are connected

	public PartWrapper(Wire top, Wire right, Part node)
	{
        _wires = new Dictionary<Vector2Int, Wire>
        {
            { Vector2Int.up, top },
            { Vector2Int.right, right }
        };
        this._node = node;
		this._isConnected = false;
	}

	public PartWrapper()
	{
        _wires = new Dictionary<Vector2Int, Wire> {{Vector2Int.up, null}, {Vector2Int.right, null}};
        this._node = null;
		this._isConnected = false;
	}

	public Wire GetWire(Vector2Int direction)
	{
		return _wires[direction];
	}

	public Part GetNode()
	{
		return _node;
	}

	public Wire[] GetWires()
	{
		return new Wire[] { _wires[Vector2Int.up], _wires[Vector2Int.right] };
	}

	public Part[] GetParts()
	{
		return new Part[] { _wires[Vector2Int.up], _wires[Vector2Int.right], _node };
	}

	public bool IsConnected()
	{ // should this be implicitly equal to node != null??
		return _isConnected;
	}

	public void SetWire(Wire wire, Vector2Int direction)
	{
		if (direction.x + direction.y == 1)
		{ // loose check for (0, 1) and (1, 0)
			_wires[direction] = wire;
		}
	}

	public void SetNode(Part node)
	{
		this._node = node;
	}

	public void SetConnected(bool connected)
	{
		this._isConnected = connected;
	}

}