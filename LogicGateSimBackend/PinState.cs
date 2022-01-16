using System;
using System.Collections.Generic;

namespace LogicGateSimBackend
{
    public class PinState
    {
        public int Id { get; }
        public bool State { get; set; }
        public List<Pin> Pins { get => pins; }
        private static int nextId = 0;
        private readonly List<Pin> pins;
        public Action<bool> OnStateUpdate;

        public PinState()
        {
            Id = GetNextId();
            pins = new List<Pin>();
        }

        public void AddPin(Pin pin)
        {
            pins.Add(pin);
        }

        public void RemovePin(Pin pin)
        {
            pins.Remove(pin);
        }

        public void OnPinStateUpdate()
        {
            foreach (var pin in pins)
            {
                if (!pin.InternalState) continue;
                State = true;
                return;
            }
            State = false;
        }

        // connects all pins with pin state b to pin state a
        public static void Connect(PinState a, PinState b)
        {
            if (a == b) throw new ArgumentException("Pin states are the same");
            var pinsInB = b.pins;
            while (pinsInB.Count > 0)
            {
                Pin.Connect(pinsInB[0], a);
            }
            // TODO: verify that pin state b is gc'd
        } 

        private static int GetNextId()
        {
            int id = nextId;
            nextId++;
            return id;
        }
    }
}