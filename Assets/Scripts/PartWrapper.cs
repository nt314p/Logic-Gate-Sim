 using System.Collections.Generic;
 using UnityEngine;

 public class PartWrapper {

     private Dictionary<Vector2Int, Wire> wires;
     private Part node;
     private bool isConnected; // whether or not the horizontal and vertical wires are connected

     public PartWrapper (Wire top, Wire right, Part node) {
         wires = new Dictionary<Vector2Int, Wire> ();
         wires.Add (Vector2Int.up, top);
         wires.Add (Vector2Int.right, right);
         this.node = node;
         this.isConnected = false;
     }

     public PartWrapper () {
         wires = new Dictionary<Vector2Int, Wire> ();
         wires.Add (Vector2Int.up, null);
         wires.Add (Vector2Int.right, null);
         this.node = null;
         this.isConnected = false;
     }

     public Wire GetWire (Vector2Int direction) {
         return wires[direction];
     }

     public Part GetNode () {
         return node;
     }

     public Wire[] GetWires () {
         return new Wire[] { wires[Vector2Int.up], wires[Vector2Int.right] };
     }

     public Part[] GetParts () {
         return new Part[] { wires[Vector2Int.up], wires[Vector2Int.right], node };
     }

     public bool IsConnected () { // should this be implicitly equal to node != null??
         return isConnected;
     }

     public void SetWire (Wire wire, Vector2Int direction) {
         if (direction.x + direction.y == 1) { // loose check for (0, 1) and (1, 0)
             wires[direction] = wire;
         }
     }

     public void SetNode (Part node) {
         this.node = node;
     }

     public void SetConnected (bool connected) {
         this.isConnected = connected;
     }

 }