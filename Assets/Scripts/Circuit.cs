using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Circuit
{
    private PartGrid _partGrid;

	public Circuit(int gridWidth, int gridHeight)
	{
		_partGrid = new PartGrid(gridWidth, gridHeight);
    }

    public void AddWires(List<Wire> wires)
    {
        _partGrid.AddWires(wires);
    }

    public void AddPart(Part part)
    {
        _partGrid.AddPart(part);
    }
}