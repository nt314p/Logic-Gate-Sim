using System.Collections;
using UnityEngine;

public class PartCollection : IEnumerable {
  private PartWrapper[, ] partsGrid;

  public PartWrapper this [Vector2Int coord] {
    get { return partsGrid[coord.x, coord.y]; }
    set { partsGrid[coord.x, coord.y] = value; }
  }

  public IEnumerator GetEnumerator () {
    return partsGrid.GetEnumerator ();
  }

  IEnumerator IEnumerable.GetEnumerator () {
    return this.GetEnumerator ();
  }
}