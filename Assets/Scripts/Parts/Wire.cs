using UnityEngine;

namespace LogicGateSimulator.Parts
{
    public class Wire : Part
    {
        public Vector2Int EndPoint { get; }

        public Vector2Int Orientation { get; }

        public Wire(Vector2Int start, Vector2Int end, int id = -1) : base(false)
        {
            this.State = false;
            this.Id = id;

            if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x))
            {
                var tmp = start; // swapping if start is not the min value
                start = end;
                end = tmp;
            }
            
            this.Coordinates = start;
            EndPoint = end;
            Orientation = end - start;
        }
    }
}