using System.Collections.Generic;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.Circuits
{
    public class DiscreteCircuit
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

        private void PartStateChanged(Part part)
        {
            if (!part.Active) return;
            _partGrid.SetPartsOfId(part.Id, part.State);
        }
        
        public DiscreteCircuit(int gridWidth, int gridHeight)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _partGrid = new PartGrid(gridWidth, gridHeight);
        }

        public void AddWires(List<Wire> wires, List<Vector2Int> wirePathCoordinates)
        {
            wires.ForEach(wire => wire.StateChanged += PartStateChanged);
            _partGrid.AddWires(wires, wirePathCoordinates);
        }

        public void AddPart(Part part)
        {
            part.StateChanged += PartStateChanged;
            _partGrid.AddPart(part);
        }
    }
}