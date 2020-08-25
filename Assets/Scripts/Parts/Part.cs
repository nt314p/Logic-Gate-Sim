using System;
using UnityEngine;

namespace LogicGateSimulator.Parts
{
    [Serializable]
    public abstract class Part
    {
        private bool _state;
        public event Action<Part> StateChanged;

        public Part(bool isActive, int id = -1)
        {
            this.Active = isActive;
            this.State = false;
            this.Id = id;
        }

        public int Id { get; set; }

        public bool State
        {
            get => this._state;
            set
            {
                if (this._state != value) StateChanged?.Invoke(this);
                this._state = value;
            }
        }
        
        public Vector2Int Coordinates { get; set; }

        public bool Active { get; }

        public override string ToString()
        {
            return $"Type: {this.GetType().Name}; Id: {this.Id}; Coords: {this.Coordinates}";
        }
    }
}