using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Part
{
    private Vector2Int _endPoint;
    private readonly Vector2Int _orientation; // either V2.up or V2.right

    public Wire(Vector2Int start, Vector2Int end, int id = -1) : base(false)
    {
        this.State = false;
        this.Coordinates = start;
        this.Id = id;

        if ((start.x > end.x && start.y == end.y) || (start.y > end.y && start.x == end.x))
        {
            var tmp = start; // swapping if start is not the min value
            start = end;
            end = tmp;
        }

        _endPoint = end;
        _orientation = end - start;
    }

    public Vector2Int GetEndPoint()
    {
        return _endPoint;
    }

    public Vector2Int Orientation { get => _orientation; }



    public bool Equals(Wire w)
    {
        return this.Coordinates.Equals(w.Coordinates) && this._endPoint.Equals(w._endPoint);
    }
}