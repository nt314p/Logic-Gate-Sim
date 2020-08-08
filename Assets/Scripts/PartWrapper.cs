using System.Collections.Generic;
using UnityEngine;

public class PartWrapper
{

	private readonly Dictionary<Vector2Int, Wire> _wires;

    public Part Node { get; set; }
    public bool Connected { get; set; }

    public PartWrapper(Wire top, Wire right, Part node)
	{
        _wires = new Dictionary<Vector2Int, Wire>
        {
            { Vector2Int.up, top },
            { Vector2Int.right, right }
        };
        this.Node = node;
		this.Connected = false;
	}

	public PartWrapper()
	{
        _wires = new Dictionary<Vector2Int, Wire> {{Vector2Int.up, null}, {Vector2Int.right, null}};
        this.Node = null;
		this.Connected = false;
	}

	public Wire GetWire(Vector2Int direction)
	{
		return _wires[direction];
	}

    public Wire[] GetWires()
	{
		return new Wire[] { _wires[Vector2Int.up], _wires[Vector2Int.right] };
	}

	public Part[] GetParts()
	{
		return new Part[] { _wires[Vector2Int.up], _wires[Vector2Int.right], Node };
	}

    public void SetWire(Wire wire, Vector2Int direction)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.right) // check for (0, 1) and (1, 0)
        {
            _wires[direction] = wire;
        }
    }
}