using System;
using System.Collections.Generic;
using System.Linq;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.Circuits
{
    public class PartGrid
    {
        private readonly PartWrapper[,] _partGrid;
        private readonly IDictionary<int, List<Part>> _partDictionary;
        private static readonly List<Vector2Int> Directions = new List<Vector2Int> { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        private int _nextAvailableId;
        private readonly int _gridWidth;
        private readonly int _gridHeight;

        public PartGrid(int gridWidth, int gridHeight)
        {
            _nextAvailableId = 0;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _partDictionary = new Dictionary<int, List<Part>>();
            _partGrid = new PartWrapper[gridWidth, gridHeight];
            for (var i = 0; i < _partGrid.GetLength(0); i++)
            {
                for (var j = 0; j < _partGrid.GetLength(1); j++)
                {
                    _partGrid[i, j] = new PartWrapper();
                }
            }
        }

        public void TrimIds()
        {
            var nextId = 0;
            var ids = new int[_partDictionary.Keys.Count];
            _partDictionary.Keys.CopyTo(ids, 0);
            Debug.Log("Trimming Ids");
            foreach (var currentId in ids)
            {
                if (_partDictionary[currentId].Count == 0)
                {
                    _partDictionary.Remove(currentId);
                }
                else
                {
                    ReplaceId(currentId, nextId++);
                }
            }
        }

        private void RebuildDictionary() // uses the part grid to rebuild the dict
        {

        }

        // calculates the state of an id based on the states of its active components
        public void CalculateStateId(int id)
        {
            var state = false;
            foreach (var p in _partDictionary[id])
            {
                if (!p.Active || state) break; // end of active parts or state = true
                state |= p.State;
            }
            SetPartsOfId(id, state);
        }

        private List<Part> GetAllOfId(int id)
        {
            return _partDictionary[id];
        }

        private void ReplaceId(int oldId, int newId)
        {
            if (oldId == newId) return;
            _partDictionary[newId] = _partDictionary[oldId];
            _partDictionary.Remove(oldId);
            _partDictionary[newId].ForEach(p => p.Id = newId);
        }

        private void MergeId(int sourceId, int destinationId)
        {
            if (sourceId == destinationId) return;
            _partDictionary[destinationId].AddRange(_partDictionary[sourceId]);
            _partDictionary.Remove(sourceId);
            _partDictionary[destinationId].ForEach(p => p.Id = destinationId);
        }

        private int LinkIds(List<int> ids)
        {
            var finalId = ids.Count == 0 ? GetNextAvailableId() : ids[0];
            for (var index = 1; index < ids.Count; index++)
            {
                MergeId(ids[index], finalId);
            }
            return finalId;
        }

        public void AddPart(Part part)
        {
            if (!(part is Wire)) AddNode(part);
        }

        private void AddNode(Part part)
        {
            var wrapper = GetWrapper(part.Coordinates);
            if (wrapper.Node != null) return; // implement throwing exception
            var surroundingWires = GetWiresAtCoordinates(part.Coordinates).Where(wire => wire != null).ToList();
            if (surroundingWires.Count == 4 && !wrapper.Connected) // ids need to be recalculated
            {
                var newId = LinkIds(wrapper.GetWires().Select(wire => wire.Id).ToList());
                part.Id = newId;
            }
            else if(surroundingWires.Count == 0)
            {
                part.Id = GetNextAvailableId();
            }
            else
            {
                part.Id = surroundingWires[0].Id;
            }
            wrapper.Node = part;
            wrapper.Connected = true; // a node will always connect
            AddPartToDictionary(part);
        }

        public void AddWires(List<Wire> wires, List<Vector2Int> wirePathCoordinates)
        {
            for (var index = 0; index < wires.Count; index++)
            {
                var wire = wires[index];
                GetWrapper(wire.Coordinates).SetWire(wire); // adding part to the grid
                UpdateConnection(wirePathCoordinates[index], wirePathCoordinates[index + 1]);
            }

            var previousCoordinates = wirePathCoordinates[0];
            var nextCoordinates = wirePathCoordinates[1];
            var connectedIds = new HashSet<int>(); // using a set for unique ids only

            // adding the first wire's opposite direction since the for loop traversal moves in forward direction only
            // var firstWireConnections =
            //     GetWiresFromDirection(firstWire.EndPoint, firstWire.Coordinates - firstWire.EndPoint);
            // firstWireConnections.ForEach(surroundingWire => connectedIds.Add(surroundingWire.Id));
            // var firstNode = GetWrapper(firstWire.Coordinates).Node;
            // if (firstNode != null && firstNode.Id != -1) connectedIds.Add(firstNode.Id);

            connectedIds.UnionWith(GetSurroundingPartIds(previousCoordinates, nextCoordinates));
            
            for (var index = 1; index < wirePathCoordinates.Count; index++)
            {
                var currentCoordinates = wirePathCoordinates[index];
                // if (Math.Abs((currentCoordinates - previousCoordinates).magnitude - 1) > 0.001f)
                //     currentCoordinates = wires[index].EndPoint;
                
                // var surroundingWires = GetWiresFromDirection(previousCoordinates, currentCoordinates - previousCoordinates);
                // surroundingWires.ForEach(surroundingWire => connectedIds.Add(surroundingWire.Id));
                
                connectedIds.UnionWith(GetSurroundingPartIds(currentCoordinates, previousCoordinates));
                previousCoordinates = currentCoordinates;
            }

            var wireId = LinkIds(connectedIds.ToList());

            foreach (var wire in wires)
            {
                wire.Id = wireId;
                AddPartToDictionary(wire);
            }
        }

        private HashSet<int> GetSurroundingPartIds(Vector2Int targetCoordinates, Vector2Int endCoordinates)
        {
            var connectedIds = new HashSet<int>();
            var surroundingWires =
                GetWiresFromDirection(targetCoordinates, endCoordinates - targetCoordinates);
            surroundingWires.ForEach(surroundingWire => connectedIds.Add(surroundingWire.Id));
            var node = GetWrapper(targetCoordinates).Node;
            if (node != null && node.Id != -1) connectedIds.Add(node.Id);
            return connectedIds;
        }

        private void AddPartToDictionary(Part part)
        {
            var partId = part.Id;
            var partsOfId = new List<Part>();
            if (_partDictionary.ContainsKey(partId))
                partsOfId = _partDictionary[partId];
            else
                _partDictionary.Add(partId, partsOfId);

            if (part.Active)
                partsOfId.Insert(0, part); // active parts go in front
            else
                partsOfId.Add(part);

        }
        
        private void UpdateConnection(Vector2Int coordinates, Vector2Int nextCoordinates)
        {
            var wires = GetWiresAtCoordinates(coordinates).Where(wire => wire != null).ToList();
            var wireCount = wires.Count;
            // Special case: three wires -> four wires remain connected (unless the wire opposite to the wire being added is also unregistered)
            switch (wireCount)
            {
                case 0:
                case 1:
                    GetWrapper(coordinates).Connected = false;
                    break;
                case 2:
                    GetWrapper(coordinates).Connected = wires[0].Orientation != wires[1].Orientation; // connect wires when at a corner
                    break;
                case 3:
                    GetWrapper(coordinates).Connected = true;
                    break;
                case 4:
                    // if the node was connected and the opposing wire to the wire being added is not unregistered
                    if (IsConnected(coordinates) && GetWire(coordinates, coordinates - nextCoordinates).Id != -1)
                    {
                        break; // do nothing because the node should keep it's connection
                    }
                    GetWrapper(coordinates).Connected = false;
                    break;
            }
        }
        
        private List<Wire> GetWiresFromDirection(Vector2Int coordinates, Vector2Int direction, bool ignoreConnected = false)
        {
            var wires = new List<Wire>();

            if (!ignoreConnected && !IsConnected(coordinates))
            {
                wires.Add(GetWire(coordinates, -direction));
            }
            else
            {
                for (var directionIndex = RotateQuarter(direction);
                    directionIndex != direction;
                    directionIndex = RotateQuarter(directionIndex))
                {
                    wires.Add(GetWire(coordinates, directionIndex));
                }
            }

            // ignore wires that are null and with with id -1 (unregistered)
            return wires.Where(wire => wire != null && wire.Id != -1).ToList();
        }

        private List<Part> GetPartsOfId(int id) => _partDictionary[id];

        public void SetPartsOfId(int id, bool state, bool ignoreActive = false)
        {
            var editParts = GetPartsOfId(id);
            if (ignoreActive) editParts = editParts.Where(p => !p.Active).ToList(); // soft set filters out active parts
            editParts.ForEach(p => p.State = state);
        }

        private bool IsConnected(Vector2Int coordinates)
        {
            return GetWrapper(coordinates).Connected;
        }

        private List<Wire> GetWiresAtCoordinates(Vector2Int coordinates)
        {
            return Directions.Select(direction => GetWire(coordinates, direction)).ToList();
        }

        // gets a Wire based on direction from the part grid
        private Wire GetWire(Vector2Int coordinates, Vector2Int direction)
        {
            // if (coordinates.x == 0 || coordinates.y == 0) return null;
            if (direction == Vector2Int.up || direction == Vector2Int.right)
                return GetWrapper(coordinates).GetWire(direction);
            
            var shifted = coordinates + direction;
            var wrapper = GetWrapper(shifted);
            return wrapper?.GetWire(-direction);
        }

        private PartWrapper GetWrapper(Vector2Int coordinates)
        {
            var x = coordinates.x;
            var y = coordinates.y;
            if (x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight)
                return _partGrid[x, y];
            return null;
        }

        private int GetNextAvailableId()
        {
            _nextAvailableId = FindNextAvailableId();
            return _nextAvailableId++;
        }

        private int FindNextAvailableId()
        {
            var testId = _nextAvailableId;
            while (_partDictionary.ContainsKey(testId))
                testId++;
        
            return testId;
        }

        private static Vector2Int RotateQuarter(Vector2Int vector)
        {
            return new Vector2Int(-vector.y, vector.x);
        }
    }
}
