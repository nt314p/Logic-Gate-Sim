using System.Collections.Generic;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.Circuits
{
    public class Circuit
    {
        private readonly PartGrid _partGrid;

        public int GridWidth
        {
            get;
        }

        public int GridHeight
        {
            get;
        }
        
        public Circuit(int gridWidth, int gridHeight)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _partGrid = new PartGrid(gridWidth, gridHeight);
        }

        public void AddWires(List<Wire> wires, List<Vector2Int> wirePathCoordinates)
        {
            _partGrid.AddWires(wires, wirePathCoordinates);
        }

        public void AddPart(Part part)
        {
            _partGrid.AddPart(part);
        }
    }
}