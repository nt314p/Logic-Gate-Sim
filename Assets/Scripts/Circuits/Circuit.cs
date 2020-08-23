using System.Collections.Generic;
using LogicGateSimulator.Parts;

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

        public void AddWires(List<Wire> wires)
        {
            _partGrid.AddWires(wires);
        }

        public void AddPart(Part part)
        {
            _partGrid.AddPart(part);
        }
    }
}